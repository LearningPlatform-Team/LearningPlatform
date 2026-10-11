using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Domain.Entities.Identity;

public class User : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public AppLanguage PreferredLanguage { get; set; } = AppLanguage.English;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public DoctorProfile? DoctorProfile { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    public TaProfile? TaProfile { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<MyListItem> MyListItems { get; set; } = new List<MyListItem>();
    public ICollection<PlaybackLog> PlaybackLogs { get; set; } = new List<PlaybackLog>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<EventReminder> EventReminders { get; set; } = new List<EventReminder>();
}
