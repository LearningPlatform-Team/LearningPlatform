using MediatR;

namespace LearningPlatform.Application.Features.Authentication.VerifyRegistrationOtp;

public record VerifyRegistrationOtpCommand(
    string? Email,
    Guid? UserId,
    string Otp
) : IRequest<VerifyRegistrationOtpResponse>;

public record VerifyRegistrationOtpResponse(
    bool Success,
    string Message,
    string? AccessToken = null,
    string? RefreshToken = null,
    Guid? UserId = null,
    string? FullName = null,
    IList<string>? Roles = null
);
