using MediatR;

namespace LearningPlatform.Application.Features.Authentication.TokenRefresh;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResponse>;

public record RefreshTokenResponse(
    bool Success,
    string Message,
    string? AccessToken = null,
    string? RefreshToken = null
);
