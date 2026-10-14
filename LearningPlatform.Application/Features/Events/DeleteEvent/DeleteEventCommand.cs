using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.DeleteEvent
{

    public sealed record DeleteEventCommand(
        Guid EventId,
        Guid DeletedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
        ) : IRequest<Result>;
}
