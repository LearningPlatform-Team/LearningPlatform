using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.VerifyRegistrationOtp;

public class VerifyRegistrationOtpHandler : IRequestHandler<VerifyRegistrationOtpCommand, ResultT<VerifyRegistrationOtpResponse>>
{
    private readonly IOtpService _otpService;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly IApplicationDbContext _context;

    public VerifyRegistrationOtpHandler(
        IOtpService otpService,
        ITokenService tokenService,
        UserManager<User> userManager,
        IApplicationDbContext context)
    {
        _otpService = otpService;
        _tokenService = tokenService;
        _userManager = userManager;
        _context = context;
    }

    public async Task<ResultT<VerifyRegistrationOtpResponse>> Handle(VerifyRegistrationOtpCommand request, CancellationToken cancellationToken)
    {
        User? user = null;
        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        else if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
        {
            user = await _userManager.FindByIdAsync(request.UserId.Value.ToString());
        }

        if (user == null)
        {
            return ResultT<VerifyRegistrationOtpResponse>.Failure("User not found.", ErrorType.NotFound);
        }

        var isValid = await _otpService.VerifyOtpAsync(user.Id, request.Otp, cancellationToken);
        if (!isValid)
        {
            return ResultT<VerifyRegistrationOtpResponse>.Failure("Invalid or expired OTP. Please try again.", ErrorType.Validation);
        }

        user.EmailConfirmed = true;
        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = HashToken(refreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return ResultT<VerifyRegistrationOtpResponse>.Success(new VerifyRegistrationOtpResponse(
            accessToken,
            refreshToken,
            user.Id,
            user.FullName,
            roles
        ));
    }

    private static string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
