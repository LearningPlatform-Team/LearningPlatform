
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Academic
{
    public class TaProfile
    {
        public Guid UserId { get; set; }
        public Guid SupervisingDoctorId { get; set; }
        public string? Department { get; set; }

        public User User { get; set; } = null!;
        public DoctorProfile SupervisingDoctor { get; set; } = null!;
        public ICollection<OfferingTa> OfferingTas { get; set; } = new List<OfferingTa>();
    }
}
