namespace LearningPlatform.Domain.Entities.Identity;

public class Otp
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string OtpHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
    public int AttemptCount { get; set; } = 0;
    public bool IsUsed => UsedAt != null;
    public bool IsValid => !IsUsed && ExpiresAt > DateTime.UtcNow && AttemptCount < 3;

    public User User { get; set; } = null!;
}
