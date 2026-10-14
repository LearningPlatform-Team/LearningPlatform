using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.EventReminders.DeleteEventReminder
{
    public sealed record DeleteEventReminderCommand(
        Guid EventId,
        Guid UserId
    ) : IRequest<Result>;
}
