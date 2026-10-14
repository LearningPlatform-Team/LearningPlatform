namespace LearningPlatform.Application.Features.Attendance.MarkManualAttendance
{
    public sealed class MarkManualAttendanceRequestDto
    {
        public List<ManualAttendanceItemDto> Students { get; set; } = [];
    }
}
