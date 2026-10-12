using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Attendance.UpdateAttendance
{
    public sealed record UpdateAttendanceCommand(
        Guid AttendanceRecordId,
        AttendanceStatus Status,
        Guid UpdatedByUserId
        ) : IRequest<Result>;
}
