using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.VerifyRegistrationOtp;

public record VerifyRegistrationOtpCommand(
    string? Email,
    Guid? UserId,
    string Otp
) : IRequest<ResultT<VerifyRegistrationOtpResponse>>;

public record VerifyRegistrationOtpResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string FullName,
    IList<string> Roles
);
