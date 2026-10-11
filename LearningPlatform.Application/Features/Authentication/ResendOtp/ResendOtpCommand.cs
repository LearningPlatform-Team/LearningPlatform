using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ResendOtp;

public record ResendOtpCommand(
    string Email
) : IRequest<ResendOtpResponse>;

public record ResendOtpResponse(
    bool Success,
    string Message
);
