
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Content
{
    public class Book
    {
        public Guid Id { get; set; }
        public int? SubjectId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileStorageKey { get; set; } = string.Empty;
        public int UploadedByUserId { get; set; }

        public Subject? Subject { get; set; }
        public User UploadedByUser { get; set; } = null!;
    }
}
