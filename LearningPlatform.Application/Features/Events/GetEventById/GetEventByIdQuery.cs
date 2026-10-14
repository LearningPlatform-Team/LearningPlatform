using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.GetEventById
{

    public sealed record GetEventByIdQuery(
        Guid EventId,
        Guid RequestedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
    ) : IRequest<ResultT<EventDetailsDto>>;
}
