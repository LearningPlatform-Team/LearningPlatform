using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResponse>;

public record LoginResponse(
    bool Success,
    string Message,
    string? AccessToken = null,
    string? RefreshToken = null,
    Guid? UserId = null,
    string? FullName = null,
    IList<string>? Roles = null,
    bool IsEmailUnconfirmed = false
);
