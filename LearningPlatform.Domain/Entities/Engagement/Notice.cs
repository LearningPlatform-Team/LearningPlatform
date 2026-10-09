
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Engagement
{
    public class Notice
    {
        public Guid Id { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public Guid AuthorUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }

        public SubjectOffering SubjectOffering { get; set; } = null!;
        public User AuthorUser { get; set; } = null!;
    }
}
