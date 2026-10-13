using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Register;

public record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string? ConfirmPassword = null,
    string? PhoneNumber = null
) : IRequest<ResultT<RegisterResponse>>;

public record RegisterResponse(
    Guid UserId,
    string Email
);
