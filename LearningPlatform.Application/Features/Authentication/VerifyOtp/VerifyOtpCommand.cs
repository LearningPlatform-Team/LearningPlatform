using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.VerifyOtp;

public record VerifyOtpCommand(
    string Email,
    string Otp,
    Guid? UserId = null
) : IRequest<ResultT<VerifyOtpResponse>>;

public record VerifyOtpResponse(
    string ResetToken
);
