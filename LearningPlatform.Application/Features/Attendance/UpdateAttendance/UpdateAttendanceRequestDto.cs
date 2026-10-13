using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Application.Features.Attendance.UpdateAttendance
{
    public sealed class UpdateAttendanceRequestDto
    {
        public AttendanceStatus Status { get; set; }
    }
}
