
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Entities.Grading;

namespace LearningPlatform.Domain.Entities.Academic
{
    public class SubjectOffering
    {
        public Guid Id { get; set; }
        public Guid SubjectId { get; set; }
        public Guid TermId { get; set; }
        public Guid DoctorProfileId { get; set; }
        public bool IsActive { get; set; }

        // Navigation properties
        public DoctorProfile Doctor { get; set; } = null!;
        public AcademicTerm Term { get; set; } = null!;
        public Subject Subject { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
        public ICollection<ClassSession> ClassSessions { get; set; } = new List<ClassSession>();
        public ICollection<OfferingTa> OfferingTas { get; set; } = new List<OfferingTa>();
        public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();
        public ICollection<Meeting> Meetings { get; set; } = new List<Meeting>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public ICollection<Notice> Notices { get; set; } = new List<Notice>();

    }
}
