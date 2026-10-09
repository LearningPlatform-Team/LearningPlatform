using LearningPlatform.Domain.Entities.Identity;
namespace LearningPlatform.Domain.Entities.Academic
{
    public class StudentProfile
    {
        public Guid UserId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
