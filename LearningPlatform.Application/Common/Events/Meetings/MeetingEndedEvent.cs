using MediatR;
namespace LearningPlatform.Application.Common.Events.Meetings
{

    public sealed record MeetingEndedEvent(
        Guid ClassSessionId,
        DateTime EndedAtUtc
        ) : INotification;
}
