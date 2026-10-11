using MediatR;
namespace LearningPlatform.Application.Features.Attendance.EventHandlers
{
    public sealed record StudentJoinedMeetingEvent(
        Guid ClassSessionId,
        Guid StudentId,
        DateTime JoinedAtUtc
        ) : INotification;
}
