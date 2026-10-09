
namespace LearningPlatform.Domain.Entities.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string? ReplacedByTokenHash { get; set; }
        public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
    }
}
