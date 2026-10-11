using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Application.Features.Authentication.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, RegisterResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;
    private readonly Microsoft.Extensions.Logging.ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IApplicationDbContext context,
        IOtpService otpService,
        Microsoft.Extensions.Logging.ILogger<RegisterHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task<RegisterResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new RegisterResponse(false, "A user with this email already exists.");
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var existingPhoneUser = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (existingPhoneUser != null)
            {
                return new RegisterResponse(false, "A user with this phone number already exists.");
            }
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            IsActive = true,
            EmailConfirmed = false, 
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return new RegisterResponse(false, $"Registration failed: {errors}");
        }

        const string studentRole = "Student";
        if (!await _roleManager.RoleExistsAsync(studentRole))
        {
            await _roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid(),
                Name = studentRole,
                NormalizedName = studentRole.ToUpperInvariant()
            });
        }
        await _userManager.AddToRoleAsync(user, studentRole);

        var studentProfile = new StudentProfile
        {
            UserId = user.Id,
            StudentCode = $"STU-{DateTime.UtcNow:yyMMdd}-{Random.Shared.Next(1000, 9999)}"
        };
        _context.StudentProfiles.Add(studentProfile);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            await _otpService.GenerateAndStoreOtpAsync(user.Id, user.Email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification OTP email to {Email}", user.Email);
            return new RegisterResponse(
                true,
                "Account registered successfully, but failed to send verification email. Please use 'Resend OTP' to receive your code.",
                user.Id,
                user.Email
            );
        }

        return new RegisterResponse(
            true, 
            "Registration successful as Student. A verification OTP code has been sent to your email. Please verify your OTP to complete account activation.", 
            user.Id,
            user.Email
        );
    }
}
