using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Interfaces.BackgroundJobs;
using LearningPlatform.Application.Common.Options;
using LearningPlatform.Application.Features.Attendance.FinalizeMeetingAttendance;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Infrastructure.BackgroundJobs.Attendance
{
    public sealed class AttendanceFinalizationJob
        : IAttendanceFinalizationJob
    {
        private const int BatchSize = 100;

        private readonly IApplicationDbContext _context;
        private readonly ISender _sender;
        private readonly AttendanceOptions _attendanceOptions;
        private readonly ILogger<AttendanceFinalizationJob> _logger;

        public AttendanceFinalizationJob(
            IApplicationDbContext context,
            ISender sender,
            IOptions<AttendanceOptions> attendanceOptions,
            ILogger<AttendanceFinalizationJob> logger)
        {
            _context = context;
            _sender = sender;
            _attendanceOptions = attendanceOptions.Value;
            _logger = logger;
        }

        public async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            var utcNow = DateTime.UtcNow;

            var finalizationThreshold = utcNow.AddMinutes(
                -_attendanceOptions.FinalizationGracePeriodMinutes);

            var sessionIds = await _context.ClassSessions
                .AsNoTracking()
                .Where(x =>
                    x.AttendanceFinalizedAt == null &&
                    x.EndTime <= finalizationThreshold)
                .OrderBy(x => x.EndTime)
                .Select(x => x.Id)
                .Take(BatchSize)
                .ToListAsync(cancellationToken);

            foreach (var sessionId in sessionIds)
            {
                try
                {
                    var command = new FinalizeMeetingAttendanceCommand(
                        sessionId,
                        utcNow);

                    var result = await _sender.Send(
                        command,
                        cancellationToken);

                    if (!result.IsSuccess)
                    {
                        _logger.LogWarning(
                            "Background attendance finalization failed for " +
                            "class session {ClassSessionId}. Reason: {Reason}",
                            sessionId,
                            result.ErrorMessage);
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                        exception,
                        "Unexpected error while finalizing attendance for " +
                        "class session {ClassSessionId}.",
                        sessionId);
                }
            }
        }
    }
}
