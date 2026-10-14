using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Domain.Entities.Attendance
{
    public class AttendanceRecord
    {
        public Guid Id { get; set; }
        public Guid ClassSessionId { get; set; }
        public Guid EnrollmentId { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTime? MarkedAt { get; set; }
        public Guid? MarkedByUserId { get; set; }
        public AttendanceSource Source { get; set; } = AttendanceSource.Automatic;
        public ClassSession ClassSession { get; set; } = null!;
        public Enrollment Enrollment { get; set; } = null!;
        public User? MarkedByUser { get; set; } = null!;
    }
}
