
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Content
{
    public class MyListItem
    {
        public int UserId { get; set; }
        public int LectureId { get; set; }

        public User User { get; set; } = null!;
        public Lecture Lecture { get; set; } = null!;

    }
}
