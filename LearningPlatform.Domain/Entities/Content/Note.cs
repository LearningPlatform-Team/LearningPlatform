
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Domain.Entities.Content
{
    public class Note
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public NoteColor Color { get; set; } = NoteColor.Default;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
    }
}
