using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.EventReminders.CreateEventReminder
{
    public sealed record SetEventReminderCommand(
        Guid EventId,
        Guid UserId,
        int MinutesBefore,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
    ) : IRequest<Result>;
}
