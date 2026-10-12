
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Domain.Entities.Engagement
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EventType Type { get; set; } = EventType.Other;
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }

        public string? Location { get; set; }

        public Guid? SubjectOfferingId { get; set; }
        public Guid CreatedByUserId { get; set; }

        public SubjectOffering? SubjectOffering { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public ICollection<EventReminder> Reminders { get; set; } = new List<EventReminder>();
    }
}
