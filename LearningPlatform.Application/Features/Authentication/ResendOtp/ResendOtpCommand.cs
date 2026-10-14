using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ResendOtp;

public record ResendOtpCommand(
    string Email
) : IRequest<Result>;
