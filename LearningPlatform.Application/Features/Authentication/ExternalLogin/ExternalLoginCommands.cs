using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ExternalLogin;

public record GoogleLoginCommand(
    string IdToken
) : IRequest<ResultT<ExternalLoginResponse>>;

public record FacebookLoginCommand(
    string AccessToken
) : IRequest<ResultT<ExternalLoginResponse>>;

public record ExternalLoginResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string FullName,
    string? Email = null,
    string? AvatarUrl = null,
    bool IsNewUser = false,
    IList<string>? Roles = null
);
