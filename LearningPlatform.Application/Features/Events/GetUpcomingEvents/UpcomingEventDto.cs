using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Application.Features.Events.GetUpcomingEvents
{

    public sealed class UpcomingEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public EventType Type { get; set; }
        public DateTime StartsAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }
        public string? Location { get; set; }
        public bool IsGeneral { get; set; }
        public Guid? SubjectOfferingId { get; set; }
        public string? SubjectName { get; set; }
        public bool HasReminder { get; set; }
        public int? ReminderMinutesBefore { get; set; }
    }
}
