using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IApplicationDbContext _context;

    public LoginHandler(
        UserManager<User> userManager,
        ITokenService tokenService,
        IApplicationDbContext context)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _context = context;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return new LoginResponse(false, "Invalid email or password.");
        }

        if (await _userManager.IsLockedOutAsync(user))
        {
            return new LoginResponse(false, "Account is temporarily locked due to multiple failed login attempts. Please try again later.");
        }

        if (!user.IsActive)
        {
            return new LoginResponse(false, "Account is deactivated. Please contact support.");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            return new LoginResponse(false, "Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        if (!user.EmailConfirmed)
        {
            return new LoginResponse(
                false, 
                "Please verify your email address before logging in.",
                UserId: user.Id,
                IsEmailUnconfirmed: true
            );
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

        return new LoginResponse(
            true,
            "Login successful",
            accessToken,
            refreshToken,
            user.Id,
            user.FullName,
            roles
        );
    }

    private string HashToken(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
