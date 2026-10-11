using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ExternalLogin;

public record GoogleLoginCommand(
    string IdToken
) : IRequest<ExternalLoginResponse>;

public record FacebookLoginCommand(
    string AccessToken
) : IRequest<ExternalLoginResponse>;

public record ExternalLoginResponse(
    bool Success,
    string Message,
    string? AccessToken = null,
    string? RefreshToken = null,
    Guid? UserId = null,
    string? FullName = null,
    string? Email = null,
    string? AvatarUrl = null,
    bool IsNewUser = false,
    IList<string>? Roles = null
);
