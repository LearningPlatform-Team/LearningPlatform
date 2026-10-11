using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Options;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LearningPlatform.Application.Features.Attendance.RecordAttendance
{

    public sealed class RecordAttendanceCommandHandler
        : IRequestHandler<RecordAttendanceCommand, ResultT<Guid>>
    {
        private readonly IApplicationDbContext _context;
        private readonly AttendanceOptions _attendanceOptions;

        public RecordAttendanceCommandHandler(
            IApplicationDbContext context,
            IOptions<AttendanceOptions> attendanceOptions)
        {
            _context = context;
            _attendanceOptions = attendanceOptions.Value;
        }

        public async Task<ResultT<Guid>> Handle(
            RecordAttendanceCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _context.ClassSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.ClassSessionId,
                    cancellationToken);

            if (session is null)
            {
                return ResultT<Guid>.Failure(
                    "Class session was not found.",
                    ErrorType.NotFound);
            }

            if (session.AttendanceFinalizedAt.HasValue)
            {
                return ResultT<Guid>.Failure(
                    "Attendance for this session has already been finalized.",
                    ErrorType.Failure);
            }
            var earliestAllowedJoinTime = session.StartTime.AddMinutes(-_attendanceOptions.JoinBeforeStartToleranceMinutes);

            if (request.JoinedAtUtc < earliestAllowedJoinTime)
            {
                return ResultT<Guid>.Failure(
                    "The join time is outside the allowed session period.",
                    ErrorType.Failure);
            }

            if (request.JoinedAtUtc > session.EndTime)
            {
                return ResultT<Guid>.Failure(
                    "The student joined after the session had ended.",
                    ErrorType.Failure);
            }

            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.StudentId == request.StudentId &&
                         x.OfferingId == session.SubjectOfferingId &&
                         x.Status == EnrollmentStatus.Enrolled,
                    cancellationToken);

            if (enrollment is null)
            {
                return ResultT<Guid>.Failure(
                    "The student is not enrolled in this subject offering.",
                    ErrorType.NotFound);
            }

            var existingAttendanceId = await _context.AttendanceRecords
                .Where(x =>
                    x.ClassSessionId == request.ClassSessionId &&
                    x.EnrollmentId == enrollment.Id)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingAttendanceId.HasValue)
            {
                // Event processing must be idempotent.
                // Receiving the same event again is treated as success.
                return ResultT<Guid>.Success(existingAttendanceId.Value);
            }

            var lateThreshold = session.StartTime.AddMinutes(
                _attendanceOptions.LateAfterMinutes);

            var attendanceStatus = request.JoinedAtUtc <= lateThreshold
                ? AttendanceStatus.Present
                : AttendanceStatus.Late;

            var attendanceRecord = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                ClassSessionId = session.Id,
                EnrollmentId = enrollment.Id,
                Status = attendanceStatus,
                MarkedAt = request.JoinedAtUtc,
                MarkedByUserId = null
            };

            await _context.AttendanceRecords.AddAsync(
                attendanceRecord,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return ResultT<Guid>.Success(attendanceRecord.Id);
        }
    }
}
