using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Authentication.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<ResultT<LoginResponse>>;

public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string FullName,
    IList<string> Roles,
    bool IsEmailUnconfirmed = false
);
