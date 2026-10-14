using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Home.GetAdminHome
{
    public sealed record GetAdminHomeQuery(
        Guid AdminId,
        bool IsAdmin,
        int ItemsCount = 5
        ) : IRequest<ResultT<AdminHomeDto>>;
}
