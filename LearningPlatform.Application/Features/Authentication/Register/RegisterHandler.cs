using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Application.Features.Authentication.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, ResultT<RegisterResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IApplicationDbContext _context;
    private readonly IOtpService _otpService;
    private readonly ILogger<RegisterHandler> _logger;

    public RegisterHandler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IApplicationDbContext context,
        IOtpService otpService,
        ILogger<RegisterHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _otpService = otpService;
        _logger = logger;
    }

    public async Task<ResultT<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return ResultT<RegisterResponse>.Failure("A user with this email already exists.", ErrorType.Conflict);
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            var existingPhoneUser = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (existingPhoneUser != null)
            {
                return ResultT<RegisterResponse>.Failure("A user with this phone number already exists.", ErrorType.Conflict);
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
            return ResultT<RegisterResponse>.Failure($"Registration failed: {errors}", ErrorType.Validation);
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
        }

        return ResultT<RegisterResponse>.Success(new RegisterResponse(user.Id, user.Email));
    }
}
