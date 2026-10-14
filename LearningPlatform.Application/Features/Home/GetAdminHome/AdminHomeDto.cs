using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Home.GetAdminHome;

public sealed class AdminHomeDto
{
    public Guid AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public int TotalUsersCount { get; set; }
    public int TotalStudentsCount { get; set; }
    public int TotalDoctorsCount { get; set; }
    public int TotalTeachingAssistantsCount { get; set; }
    public int TotalAdminsCount { get; set; }
    public int TotalSubjectOfferingsCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public int UpcomingMeetingsCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public IReadOnlyList<AdminHomeEventDto> UpcomingGeneralEvents { get; set; } = [];
    public IReadOnlyList<AdminHomeMeetingDto> UpcomingMeetings { get; set; } = [];
    public IReadOnlyList<AdminHomeNotificationDto> RecentNotifications { get; set; } = [];
}

public sealed class AdminHomeEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public string? Location { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public bool HasReminder { get; set; }
    public int? ReminderMinutesBefore { get; set; }
}

public sealed class AdminHomeMeetingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ScheduledAtUtc { get; set; }
    public int MinutesDuration { get; set; }
    public Guid SubjectOfferingId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
}

public sealed class AdminHomeNotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}