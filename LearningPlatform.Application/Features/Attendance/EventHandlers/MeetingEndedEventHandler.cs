using LearningPlatform.Application.Common.Events.Meetings;
using LearningPlatform.Application.Features.Attendance.FinalizeMeetingAttendance;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Application.Features.Attendance.EventHandlers
{
    public sealed class MeetingEndedEventHandler
        : INotificationHandler<MeetingEndedEvent>
    {
        private readonly ISender _sender;
        private readonly ILogger<MeetingEndedEventHandler> _logger;

        public MeetingEndedEventHandler(
            ISender sender,
            ILogger<MeetingEndedEventHandler> logger)
        {
            _sender = sender;
            _logger = logger;
        }

        public async Task Handle(
            MeetingEndedEvent notification,
            CancellationToken cancellationToken)
        {
            var command = new FinalizeMeetingAttendanceCommand(
                notification.ClassSessionId,
                notification.EndedAtUtc);

            var result = await _sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError(
                    "Attendance finalization failed for class session " +
                    "{ClassSessionId}. Reason: {Reason}",
                    notification.ClassSessionId,
                    result.ErrorMessage);
            }
        }
    }
}
