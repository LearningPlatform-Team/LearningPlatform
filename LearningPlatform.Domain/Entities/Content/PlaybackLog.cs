
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Content
{
    public class PlaybackLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LectureId { get; set; }
        public DateTime RequestedAt { get; set; }

        public User User { get; set; } = null!;
        public Lecture Lecture { get; set; } = null!;
    }
}
