using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.TokenRefresh;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<ResultT<RefreshTokenResponse>>;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken
);
