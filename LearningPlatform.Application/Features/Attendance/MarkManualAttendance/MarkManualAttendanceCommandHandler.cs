using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Attendance.MarkManualAttendance;

public sealed class MarkManualAttendanceCommandHandler : IRequestHandler<MarkManualAttendanceCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public MarkManualAttendanceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(MarkManualAttendanceCommand request,CancellationToken cancellationToken)
    {
        var session = await _context.ClassSessions
            .AsNoTracking()
            .Where(session => session.Id == request.ClassSessionId)
            .Select(session => new
            {
                session.Id,
                session.SubjectOfferingId,
                session.AttendanceFinalizedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
        {
            return Result.Failure("Class session was not found.",ErrorType.NotFound);
        }

        if (session.AttendanceFinalizedAt.HasValue)
        {
            return Result.Failure("Attendance for this session has already been finalized Use attendance correction instead.",ErrorType.Failure);
        }

        var enrollmentIds = request.Students
            .Select(item => item.EnrollmentId)
            .ToList();

        var validEnrollmentIds = await _context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollmentIds.Contains(enrollment.Id) &&
                enrollment.OfferingId == session.SubjectOfferingId &&
                enrollment.Status == EnrollmentStatus.Enrolled)
            .Select(enrollment => enrollment.Id)
            .ToListAsync(cancellationToken);

        if (validEnrollmentIds.Count != enrollmentIds.Count)
        {
            return Result.Failure("One or more students are not actively enrolled in the session subject offering.",
                ErrorType.Failure);
        }

        var existingRecords =
            await _context.AttendanceRecords
                .Where(record => record.ClassSessionId == request.ClassSessionId &&
                    enrollmentIds.Contains(record.EnrollmentId))
                .ToListAsync(cancellationToken);

        var automaticRecordExists = existingRecords.Any(record => record.Source == AttendanceSource.Automatic);
        if (automaticRecordExists)
        {
            return Result.Failure("One or more attendance records were created automatically. Use attendance correction instead.",
                ErrorType.Failure);
        }

        var existingRecordsByEnrollmentId = existingRecords.ToDictionary(record => record.EnrollmentId);
        var utcNow = DateTime.UtcNow;
        foreach (var item in request.Students)
        {
            if (existingRecordsByEnrollmentId.TryGetValue(item.EnrollmentId,out var existingRecord))
            {
                existingRecord.Status = item.Status;
                existingRecord.Source = AttendanceSource.Manual;
                existingRecord.MarkedAt = item.Status == AttendanceStatus.Absent ? null : utcNow;
                existingRecord.MarkedByUserId = request.MarkedByUserId;
                continue;
            }

            var attendanceRecord = new AttendanceRecord
                {
                    Id = Guid.NewGuid(),
                    ClassSessionId = request.ClassSessionId,
                    EnrollmentId = item.EnrollmentId,
                    Status = item.Status,
                    Source = AttendanceSource.Manual,
                    MarkedAt = item.Status == AttendanceStatus.Absent ? null : utcNow,
                    MarkedByUserId = request.MarkedByUserId
                };

            await _context.AttendanceRecords.AddAsync(attendanceRecord,cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}