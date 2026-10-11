using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string? ConfirmPassword = null,
    string? PhoneNumber = null
) : IRequest<RegisterResponse>;

public record RegisterResponse(
    bool Success,
    string Message,
    Guid? UserId = null,
    string? Email = null
);
