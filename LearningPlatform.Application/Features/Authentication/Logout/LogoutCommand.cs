using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Logout;

public record LogoutCommand(
    string? RefreshToken = null,
    Guid? UserId = null
) : IRequest<LogoutResponse>;

public record LogoutResponse(
    bool Success,
    string Message
);
