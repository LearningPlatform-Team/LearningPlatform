using LearningPlatform.Application.Features.Attendance.RecordAttendance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Application.Features.Attendance.EventHandlers
{

    public sealed class StudentJoinedMeetingEventHandler
        : INotificationHandler<StudentJoinedMeetingEvent>
    {
        private readonly ISender _sender;
        private readonly ILogger<StudentJoinedMeetingEventHandler> _logger;

        public StudentJoinedMeetingEventHandler(
            ISender sender,
            ILogger<StudentJoinedMeetingEventHandler> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Handle(
            StudentJoinedMeetingEvent notification,
            CancellationToken cancellationToken)
        {
            var command = new RecordAttendanceCommand(
                notification.ClassSessionId,
                notification.StudentId,
                notification.JoinedAtUtc);

            var result = await _sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Attendance could not be recorded for student {StudentId} " +
                    "in class session {ClassSessionId}. Reason: {Reason}",
                    notification.StudentId,
                    notification.ClassSessionId,
                    result.ErrorMessage);
            }
        }
    }
}
