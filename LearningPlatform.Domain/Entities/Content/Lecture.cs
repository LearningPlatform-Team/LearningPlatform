
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Content
{
    public class Lecture
    {
        public Guid Id { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public Guid UploadedById { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public string VideoProvider { get; set; } = string.Empty; // e.g., "YouTube"
        public string VideoUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublished { get; set; }

        // Navigation Properties 
        public SubjectOffering Offering { get; set; } = null!;
        public User UploadedBy { get; set; } = null!;
        public ICollection<MyListItem> MyListItems { get; set; } = new List<MyListItem>();
        public ICollection<PlaybackLog> PlaybackLogs { get; set; } = new List<PlaybackLog>();

    }
}
