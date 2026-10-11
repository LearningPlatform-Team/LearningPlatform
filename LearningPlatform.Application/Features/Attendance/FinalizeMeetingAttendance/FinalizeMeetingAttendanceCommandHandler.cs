using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Attendance.FinalizeMeetingAttendance
{
    public sealed class FinalizeMeetingAttendanceCommandHandler
        : IRequestHandler<FinalizeMeetingAttendanceCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public FinalizeMeetingAttendanceCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(
            FinalizeMeetingAttendanceCommand request,
            CancellationToken cancellationToken)
        {
            var session = await _context.ClassSessions
                .FirstOrDefaultAsync(
                    x => x.Id == request.ClassSessionId,
                    cancellationToken);

            if (session is null)
            {
                return Result.Failure(
                    "Class session was not found.",
                    ErrorType.NotFound);
            }

            // Makes the command idempotent.
            if (session.AttendanceFinalizedAt.HasValue)
            {
                return Result.Success();
            }

            if (request.FinalizedAtUtc < session.EndTime)
            {
                return Result.Failure(
                    "Attendance cannot be finalized before the session ends.",
                    ErrorType.Failure);
            }

            var enrolledStudents = await _context.Enrollments
                .AsNoTracking()
                .Where(x =>
                    x.OfferingId == session.SubjectOfferingId &&
                    x.Status == EnrollmentStatus.Enrolled)
                .Select(x => new
                {
                    EnrollmentId = x.Id,
                    StudentId = x.StudentId
                })
                .ToListAsync(cancellationToken);

            var recordedEnrollmentIds = await _context.AttendanceRecords
                .AsNoTracking()
                .Where(x => x.ClassSessionId == session.Id)
                .Select(x => x.EnrollmentId)
                .ToListAsync(cancellationToken);

            var recordedEnrollmentIdSet = recordedEnrollmentIds.ToHashSet();

            var absentRecords = enrolledStudents
                .Where(x => !recordedEnrollmentIdSet.Contains(x.EnrollmentId))
                .Select(x => new AttendanceRecord
                {
                    Id = Guid.NewGuid(),
                    ClassSessionId = session.Id,
                    EnrollmentId = x.EnrollmentId,
                    Status = AttendanceStatus.Absent,
                    MarkedAt = null,
                    MarkedByUserId = null
                })
                .ToList();

            if (absentRecords.Count > 0)
            {
                await _context.AttendanceRecords.AddRangeAsync(
                    absentRecords,
                    cancellationToken);
            }

            session.AttendanceFinalizedAt = request.FinalizedAtUtc;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
