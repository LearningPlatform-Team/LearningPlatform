using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.GetUpcomingEvents
{

    public sealed record GetUpcomingEventsQuery(
        Guid RequestedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA,
        int Take = 5
        ) : IRequest<ResultT<IReadOnlyList<UpcomingEventDto>>>;
}
