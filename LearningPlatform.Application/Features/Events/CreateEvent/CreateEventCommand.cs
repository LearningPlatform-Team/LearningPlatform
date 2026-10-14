using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.CreateEvent
{
    public sealed record CreateEventCommand(
        string Title,
        string? Description,
        EventType Type,
        DateTime StartsAtUtc,
        DateTime? EndsAtUtc,
        string? Location,
        Guid? SubjectOfferingId,
        Guid CreatedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
    ) : IRequest<ResultT<Guid>>;
}
