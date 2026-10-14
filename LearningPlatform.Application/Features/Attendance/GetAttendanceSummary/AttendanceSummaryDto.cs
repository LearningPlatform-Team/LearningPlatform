namespace LearningPlatform.Application.Features.Attendance.GetAttendanceSummary
{
    public sealed class AttendanceSummaryDto
    {
        public Guid EnrollmentId { get; set; }
        public Guid StudentId { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TotalSessions { get; set; }
        public int EarlyCount { get; set; }
        public int LateCount { get; set; }
        public int AbsentCount { get; set; }
        public decimal AttendancePercentage { get; set; }
        public decimal AbsencePercentage { get; set; }
    }
}
