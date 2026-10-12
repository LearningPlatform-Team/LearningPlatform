using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.UpdateEvent
{
    public sealed record UpdateEventCommand(
        Guid EventId,
        string Title,
        string? Description,
        EventType Type,
        DateTime StartsAtUtc,
        DateTime? EndsAtUtc,
        string? Location,
        Guid? SubjectOfferingId,
        Guid UpdatedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
    ) : IRequest<Result>;
}
