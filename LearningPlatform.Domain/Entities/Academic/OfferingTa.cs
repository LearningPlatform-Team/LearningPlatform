
namespace LearningPlatform.Domain.Entities.Academic
{
    public class OfferingTa
    {
        public Guid OfferingId { get; set; }
        public Guid TaProfileId { get; set; }
        public DateTime AssignedAt { get; set; }
        public SubjectOffering Offering { get; set; } = null!;
        public TaProfile TaProfile { get; set; } = null!;
    }
}
