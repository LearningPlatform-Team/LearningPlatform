
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Academic
{
    public class DoctorProfile
    {
        public Guid UserId { get; set; }
        public string? AcademicTitle { get; set; }
        public User User { get; set; } = null!;
        public ICollection<SubjectOffering> Offerings { get; set; } = new List<SubjectOffering>();
    }
}
