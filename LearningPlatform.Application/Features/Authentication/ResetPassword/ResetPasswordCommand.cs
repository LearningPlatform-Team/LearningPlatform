using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword,
    string? ConfirmPassword = null,
    Guid? UserId = null
) : IRequest<Result>;
