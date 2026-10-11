using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Domain.Entities.Identity
{
    public class User 
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public AppLanguage PreferredLanguage { get; set; } = AppLanguage.English;
        public UserType UserType { get; set; } = UserType.Student;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //Identity Framework navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Role profiles (a user normally has exactly one of these)
        public DoctorProfile? DoctorProfile { get; set; }
        public StudentProfile? StudentProfile { get; set; }
        public TaProfile? TaProfile { get; set; }

        // Personal space
        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<MyListItem> MyListItems { get; set; } = new List<MyListItem>();
        public ICollection<PlaybackLog> PlaybackLogs { get; set; } = new List<PlaybackLog>();

        // Engagement
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<EventReminder> EventReminders { get; set; } = new List<EventReminder>();

    }
}
