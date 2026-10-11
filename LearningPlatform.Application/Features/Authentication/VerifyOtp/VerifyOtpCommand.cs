using MediatR;

namespace LearningPlatform.Application.Features.Authentication.VerifyOtp;

public record VerifyOtpCommand(
    string Email,
    string Otp,
    Guid? UserId = null
) : IRequest<VerifyOtpResponse>;

public record VerifyOtpResponse(
    bool Success,
    string Message,
    string? ResetToken = null
);
