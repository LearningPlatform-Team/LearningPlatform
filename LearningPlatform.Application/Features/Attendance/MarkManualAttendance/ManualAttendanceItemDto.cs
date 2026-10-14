using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Application.Features.Attendance.MarkManualAttendance
{
    public sealed class ManualAttendanceItemDto
    {
        public Guid EnrollmentId { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}
