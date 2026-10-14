using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Home.GetTeachingStaffHome
{
    public sealed record GetTeachingStaffHomeQuery(
        Guid UserId,
        bool IsDoctor,
        bool IsTA,
        int ItemsCount = 5
         ) : IRequest<ResultT<TeachingStaffHomeDto>>;
}
