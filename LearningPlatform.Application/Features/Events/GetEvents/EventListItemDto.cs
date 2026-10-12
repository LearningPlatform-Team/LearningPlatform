using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Application.Features.Events.GetEvents
{
    public sealed class EventListItemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EventType Type { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public string? Location { get; set; }
        public bool IsGeneral { get; set; }
        public Guid? SubjectOfferingId { get; set; }
        public string? SubjectName { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public bool HasReminder { get; set; }
        public int? ReminderMinutesBefore { get; set; }
    }
}
