using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LearningPlatform.Infrastructure.Security;

public class OtpService : IOtpService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public OtpService(
        IApplicationDbContext context,
        UserManager<User> userManager,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<string> GenerateAndStoreOtpAsync(Guid userId, string email, CancellationToken cancellationToken = default)
    {
        var expiryMinutes = _configuration.GetValue<int>("OtpSettings:ExpiryMinutes", 5);
        var otpLength = _configuration.GetValue<int>("OtpSettings:Length", 4);
        var cooldownSeconds = _configuration.GetValue<int>("OtpSettings:CooldownSeconds", 60);

        // Check cooldown to prevent email spamming
        var recentOtp = await _context.Otps
            .Where(o => o.UserId == userId && o.CreatedAt > DateTime.UtcNow.AddSeconds(-cooldownSeconds))
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (recentOtp != null)
        {
            var remainingSeconds = (int)Math.Ceiling((recentOtp.CreatedAt.AddSeconds(cooldownSeconds) - DateTime.UtcNow).TotalSeconds);
            if (remainingSeconds > 0)
            {
                throw new InvalidOperationException($"Please wait {remainingSeconds} seconds before requesting a new code.");
            }
        }

        await InvalidateOtpAsync(userId, cancellationToken);

        var otp = GenerateRandomOtp(otpLength);
        var otpHash = HashOtp(otp);

        var otpEntity = new Otp
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PhoneNumber = email,
            OtpHash = otpHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes)
        };

        _context.Otps.Add(otpEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        await _emailService.SendOtpEmailAsync(email, otp, user?.FullName ?? "User", cancellationToken);

        return otp;
    }

    public async Task<bool> VerifyOtpAsync(Guid userId, string otp, CancellationToken cancellationToken = default)
    {
        var otpEntity = await _context.Otps
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpEntity == null || !otpEntity.IsValid)
        {
            return false;
        }

        var otpHash = HashOtp(otp);
        if (otpEntity.OtpHash != otpHash)
        {
            otpEntity.AttemptCount++;
            await _context.SaveChangesAsync(cancellationToken);
            return false;
        }

        otpEntity.UsedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task InvalidateOtpAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeOtps = await _context.Otps
            .Where(o => o.UserId == userId && o.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var otp in activeOtps)
        {
            otp.UsedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateRandomOtp(int length)
    {
        const string digits = "0123456789";
        var otp = new char[length];
        for (int i = 0; i < length; i++)
        {
            otp[i] = digits[System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, digits.Length)];
        }
        return new string(otp);
    }

    private static string HashOtp(string otp)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(otp);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
