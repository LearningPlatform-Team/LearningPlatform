
using LearningPlatform.Domain.Entities.Identity;

namespace LearningPlatform.Domain.Entities.Engagement
{
    public class EventReminder
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public int MinutesBefore { get; set; }

        public Event Event { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
