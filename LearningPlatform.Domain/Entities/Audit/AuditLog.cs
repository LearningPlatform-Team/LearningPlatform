
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Audit
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public Guid ChangedById { get; set; }
        public User ChangedBy { get; set; } = null!;

    }
}
