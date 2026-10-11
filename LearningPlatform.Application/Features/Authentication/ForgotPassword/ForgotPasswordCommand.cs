using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ForgotPassword;

public record ForgotPasswordCommand(
    string Email
) : IRequest<ForgotPasswordResponse>;

public record ForgotPasswordResponse(
    bool Success,
    string Message
);
