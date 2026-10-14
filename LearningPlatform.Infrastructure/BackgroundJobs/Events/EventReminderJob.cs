using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Interfaces.BackgroundJobs;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Infrastructure.BackgroundJobs.Events;

public sealed class EventReminderJob : IEventReminderJob
{
    private const int BatchSize = 100;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<EventReminderJob> _logger;

    public EventReminderJob(IApplicationDbContext context,ILogger<EventReminderJob> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        _logger.LogInformation("Event reminder job started at {StartedAtUtc}.",utcNow);
        var dueReminders = await _context.EventReminders
            .Where(reminder =>
                reminder.SentAtUtc == null &&
                reminder.Event.StartsAtUtc > utcNow &&
                reminder.Event.StartsAtUtc.AddMinutes(-reminder.MinutesBefore) <= utcNow)
            .OrderBy(reminder => reminder.Event.StartsAtUtc)
            .ThenBy(reminder => reminder.EventId)
            .Take(BatchSize)
            .Select(reminder => new
            {
                Reminder = reminder,
                EventTitle = reminder.Event.Title,
                EventStartsAtUtc = reminder.Event.StartsAtUtc
            })
            .ToListAsync(cancellationToken);
        if (dueReminders.Count == 0)
        {
            _logger.LogInformation("Event reminder job completed with no due reminders.");
            return;
        }

        var notifications = new List<Notification>(dueReminders.Count);
        foreach (var dueReminder in dueReminders)
        {
            var reminder = dueReminder.Reminder;
            notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = reminder.UserId,
                Type = NotificationType.Event,
                Title = dueReminder.EventTitle,
                Body = BuildNotificationBody(dueReminder.EventTitle,dueReminder.EventStartsAtUtc,reminder.MinutesBefore),
                IsRead = false,
                CreatedAt = utcNow
            });
            reminder.SentAtUtc = utcNow;
        }

        await _context.Notifications.AddRangeAsync(notifications,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Event reminder job created {NotificationCount} notifications.",notifications.Count);
    }

    private static string BuildNotificationBody(string eventTitle,DateTime eventStartsAtUtc,int minutesBefore)
    {
        return$"The event '{eventTitle}' starts in " +
              $"{minutesBefore} minute(s) at " +
              $"{eventStartsAtUtc:yyyy-MM-dd HH:mm} UTC.";
    }
}