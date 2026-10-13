using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Application.Features.EventReminders.GetEventReminders
{
    public sealed class EventReminderDto
    {
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public EventType EventType { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public string? Location { get; set; }
        public int MinutesBefore { get; set; }
        public DateTime ReminderAtUtc { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public bool IsGeneral { get; set; }
        public Guid? SubjectOfferingId { get; set; }
        public string? SubjectName { get; set; }
    }
}
