using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.ExternalLogin;

public class GoogleLoginHandler : IRequestHandler<GoogleLoginCommand, ExternalLoginResponse>
{
    private readonly IExternalAuthService _externalAuthService;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _context;

    public GoogleLoginHandler(
        IExternalAuthService externalAuthService,
        UserManager<User> userManager,
        ITokenService tokenService,
        IApplicationDbContext context)
    {
        _externalAuthService = externalAuthService;
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<ExternalLoginResponse> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate the Google ID token
        var userInfo = await _externalAuthService.ValidateGoogleTokenAsync(request.IdToken, cancellationToken);

        if (userInfo == null)
        {
            return new ExternalLoginResponse(false, "Invalid Google token. Please try again.");
        }

        // 2. Find or create user
        var (user, isNewUser) = await FindOrCreateUserAsync(userInfo, "Google", cancellationToken);

        if (user == null)
        {
            return new ExternalLoginResponse(false, "Failed to create account. Please try again.");
        }

        // 3. Generate JWT tokens
        return await GenerateLoginResponseAsync(user, isNewUser, cancellationToken);
    }

    private async Task<(User? user, bool isNewUser)> FindOrCreateUserAsync(
        ExternalUserInfo userInfo, string provider, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(userInfo.Email);

        if (user != null)
        {
            // User exists — update avatar if available and link provider
            if (!string.IsNullOrEmpty(userInfo.AvatarUrl) && string.IsNullOrEmpty(user.AvatarUrl))
            {
                user.AvatarUrl = userInfo.AvatarUrl;
                await _userManager.UpdateAsync(user);
            }

            // Check if this provider login is already linked
            var existingLogin = await _userManager.FindByLoginAsync(provider, userInfo.ProviderId);
            if (existingLogin == null)
            {
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userInfo.ProviderId, provider));
            }

            return (user, false);
        }

        // Create new user
        user = new User
        {
            Id = Guid.NewGuid(),
            FullName = userInfo.FullName,
            Email = userInfo.Email,
            UserName = userInfo.Email,
            AvatarUrl = userInfo.AvatarUrl ?? string.Empty,
            IsActive = true,
            EmailConfirmed = true, // Email verified by Google/Facebook
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            return (null, false);
        }

        // Link external login
        await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userInfo.ProviderId, provider));

        // Assign Student role
        var roleExists = await _context.Roles.AnyAsync(r => r.Name == "Student", cancellationToken);
        if (roleExists)
        {
            await _userManager.AddToRoleAsync(user, "Student");
        }

        // Create Student Profile
        var hasProfile = await _context.StudentProfiles.AnyAsync(sp => sp.UserId == user.Id, cancellationToken);
        if (!hasProfile)
        {
            var studentProfile = new LearningPlatform.Domain.Entities.Academic.StudentProfile
            {
                UserId = user.Id,
                StudentCode = $"STU-{DateTime.UtcNow:yyMMdd}-{Random.Shared.Next(1000, 9999)}"
            };
            _context.StudentProfiles.Add(studentProfile);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return (user, true);
    }

    private async Task<ExternalLoginResponse> GenerateLoginResponseAsync(
        User user, bool isNewUser, CancellationToken cancellationToken)
    {
        if (!user.IsActive)
        {
            return new ExternalLoginResponse(false, "Account is deactivated. Please contact support.");
        }

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

        return new ExternalLoginResponse(
            true,
            isNewUser ? "Account created successfully." : "Login successful.",
            accessToken,
            refreshToken,
            user.Id,
            user.FullName,
            user.Email,
            user.AvatarUrl,
            isNewUser,
            roles
        );
    }

    private static string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
