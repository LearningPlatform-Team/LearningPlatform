
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Entities.Grading;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities.Academic
{
    public class Enrollment
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid OfferingId { get; set; }
        public Guid EnrolledBy { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

        // Navigation properties
        public StudentProfile Student { get; set; } = null!;
        public SubjectOffering SubjectOffering { get; set; } = null!;
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
