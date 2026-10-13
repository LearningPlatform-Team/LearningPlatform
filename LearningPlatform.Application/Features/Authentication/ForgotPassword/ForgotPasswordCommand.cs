using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.ForgotPassword;

public record ForgotPasswordCommand(
    string Email
) : IRequest<Result>;
