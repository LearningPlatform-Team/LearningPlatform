using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Logout;

public record LogoutCommand(
    string? RefreshToken = null,
    Guid? UserId = null
) : IRequest<Result>;
