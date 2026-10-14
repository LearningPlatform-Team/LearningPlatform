using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Home.GetTeachingStaffHome;
public sealed class TeachingStaffHomeDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int UpcomingAcademicEventsCount { get; set; }
    public int UpcomingMeetingsCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public IReadOnlyList<TeachingStaffHomeEventDto> UpcomingEvents { get; set; } = [];
    public IReadOnlyList<TeachingStaffHomeMeetingDto> UpcomingMeetings { get; set; } = [];
    public IReadOnlyList<TeachingStaffHomeNotificationDto> RecentNotifications { get; set; } = [];
}

public sealed class TeachingStaffHomeEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public string? Location { get; set; }
    public Guid SubjectOfferingId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool HasReminder { get; set; }
    public int? ReminderMinutesBefore { get; set; }
}

public sealed class TeachingStaffHomeMeetingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ScheduledAtUtc { get; set; }
    public int MinutesDuration { get; set; }
    public Guid SubjectOfferingId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string? JoinUrl { get; set; }
}

public sealed class TeachingStaffHomeNotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}