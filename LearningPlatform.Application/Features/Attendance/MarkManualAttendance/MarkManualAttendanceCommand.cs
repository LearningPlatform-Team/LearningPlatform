using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Attendance.MarkManualAttendance
{
    public sealed record ManualAttendanceItem(Guid EnrollmentId,AttendanceStatus Status);

    public sealed record MarkManualAttendanceCommand(
        Guid ClassSessionId,
        IReadOnlyCollection<ManualAttendanceItem> Students,
        Guid MarkedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA
    ) : IRequest<Result>;
}
