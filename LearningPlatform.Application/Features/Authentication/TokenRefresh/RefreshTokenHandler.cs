using System.Security.Cryptography;
using System.Text;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Authentication.TokenRefresh;

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;

    public RefreshTokenHandler(
        IApplicationDbContext context,
        ITokenService tokenService,
        UserManager<User> userManager)
    {
        _context = context;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = HashToken(request.RefreshToken);

        var existingRefreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);

        if (existingRefreshToken == null)
        {
            return new RefreshTokenResponse(false, "Invalid or expired refresh token.");
        }

        if (existingRefreshToken.RevokedAt != null)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == existingRefreshToken.UserId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new RefreshTokenResponse(false, "Revoked refresh token detected. Please log in again.");
        }

        if (existingRefreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return new RefreshTokenResponse(false, "Refresh token has expired. Please log in again.");
        }

        var user = existingRefreshToken.User;
        if (user == null || !user.IsActive)
        {
            return new RefreshTokenResponse(false, "User account is inactive.");
        }

        existingRefreshToken.RevokedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, user.FullName, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            TokenHash = HashToken(newRefreshToken),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        existingRefreshToken.ReplacedByTokenHash = newRefreshTokenEntity.TokenHash;

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(
            true,
            "Token refreshed successfully.",
            newAccessToken,
            newRefreshToken
        );
    }

    private static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
