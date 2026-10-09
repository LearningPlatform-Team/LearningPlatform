
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities.Engagement
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; } = NotificationType.General;
        public string Title { get; set; } = string.Empty;
        public string? Body { get; set; }

        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
