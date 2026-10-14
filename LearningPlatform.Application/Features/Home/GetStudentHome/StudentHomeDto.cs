using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Home.GetStudentHome;

public sealed class StudentHomeDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int ActiveEnrollmentsCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public int UpcomingEventsCount { get; set; }
    public decimal AttendancePercentage { get; set; }
    public IReadOnlyList<StudentHomeEventDto> UpcomingEvents { get; set; } = [];
    public IReadOnlyList<StudentHomeMeetingDto> UpcomingMeetings { get; set; } = [];
    public IReadOnlyList<StudentHomeNotificationDto> RecentNotifications { get; set; } = [];
}

public sealed class StudentHomeEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType Type { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public string? Location { get; set; }
    public string? SubjectName { get; set; }
    public bool HasReminder { get; set; }
    public int? ReminderMinutesBefore { get; set; }
}

public sealed class StudentHomeMeetingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? ScheduledAtUtc { get; set; }
    public int MinutesDuration { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string JoinUrl { get; set; } = string.Empty;
}

public sealed class StudentHomeNotificationDto
{
    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}