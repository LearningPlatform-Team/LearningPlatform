
using LearningPlatform.Domain.Entities.Academic;

namespace LearningPlatform.Domain.Entities.Attendance
{
    public class ClassSession
    {
        public Guid Id { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? AttendanceFinalizedAt { get; set; }

        public SubjectOffering SubjectOffering { get; set; } = null!;
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
