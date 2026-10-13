using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LearningPlatform.Application.Features.Attendance.GetAttendanceSummary;

public sealed class GetAttendanceSummaryQueryHandler : IRequestHandler<GetAttendanceSummaryQuery,ResultT<AttendanceSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAttendanceSummaryQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<AttendanceSummaryDto>> Handle(GetAttendanceSummaryQuery request,CancellationToken cancellationToken)
    {
        var enrollment = await _context.Enrollments
            .AsNoTracking()
            .Where(x => x.Id == request.EnrollmentId)
            .Select(x => new
            {
                EnrollmentId = x.Id,
                x.StudentId,
                SubjectOfferingId = x.OfferingId,
                SubjectName = x.SubjectOffering.Subject.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
        {
            return ResultT<AttendanceSummaryDto>.Failure(
                "Enrollment was not found.",
                ErrorType.NotFound);
        }

        var attendanceCounts = await _context.AttendanceRecords
            .AsNoTracking()
            .Where(x => x.EnrollmentId == request.EnrollmentId)
            .GroupBy(x => x.EnrollmentId)
            .Select(group => new
            {
                TotalSessions = group.Count(),
                EarlyCount = group.Count(x =>
                    x.Status == AttendanceStatus.Present),
                LateCount = group.Count(x =>
                    x.Status == AttendanceStatus.Late),
                AbsentCount = group.Count(x =>
                    x.Status == AttendanceStatus.Absent)
            })
            .FirstOrDefaultAsync(cancellationToken);
        var totalSessions = attendanceCounts?.TotalSessions ?? 0;
        var earlyCount = attendanceCounts?.EarlyCount ?? 0;
        var lateCount = attendanceCounts?.LateCount ?? 0;
        var absentCount = attendanceCounts?.AbsentCount ?? 0;
        var attendedSessions = earlyCount + lateCount;
        var attendancePercentage = totalSessions == 0
            ? 0 : Math.Round(attendedSessions * 100m / totalSessions, 2);

        var absencePercentage = totalSessions == 0
            ? 0 : Math.Round(absentCount * 100m / totalSessions, 2);

        var summary = new AttendanceSummaryDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            StudentId = enrollment.StudentId,
            SubjectOfferingId = enrollment.SubjectOfferingId,
            SubjectName = enrollment.SubjectName,
            TotalSessions = totalSessions,
            EarlyCount = earlyCount,
            LateCount = lateCount,
            AbsentCount = absentCount,
            AttendancePercentage = attendancePercentage,
            AbsencePercentage = absencePercentage
        };

        return ResultT<AttendanceSummaryDto>.Success(summary);
    }
}