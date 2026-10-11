
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Content
{
    public class MyListItem
    {
        public Guid UserId { get; set; }
        public Guid LectureId { get; set; }

        public User User { get; set; } = null!;
        public Lecture Lecture { get; set; } = null!;

    }
}
