using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Home.GetTeachingStaffHome;

public sealed class GetTeachingStaffHomeQueryHandler : IRequestHandler<GetTeachingStaffHomeQuery,ResultT<TeachingStaffHomeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetTeachingStaffHomeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<TeachingStaffHomeDto>> Handle(GetTeachingStaffHomeQuery request,CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var user = await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == request.UserId)
            .Select(user => new
            {
                user.Id,
                user.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return ResultT<TeachingStaffHomeDto>.Failure("Teaching staff user was not found.",ErrorType.NotFound);
        }

        /*
         * Academic events have a SubjectOfferingId.
         * General events are created by Admin users, so they
         * are not included in the teaching staff statistics.
         */
        var upcomingEventsQuery = _context.Events.AsNoTracking()
            .Where(eventItem => eventItem.CreatedByUserId == request.UserId &&
                eventItem.SubjectOfferingId != null && eventItem.StartsAtUtc >= utcNow);
        var upcomingAcademicEventsCount = await upcomingEventsQuery.CountAsync(cancellationToken);
        var upcomingEvents = await upcomingEventsQuery
            .OrderBy(eventItem => eventItem.StartsAtUtc)
            .ThenBy(eventItem => eventItem.Id)
            .Take(request.ItemsCount)
            .Select(eventItem => new TeachingStaffHomeEventDto
                {
                    Id = eventItem.Id,
                    Title = eventItem.Title,
                    Type = eventItem.Type,
                    StartsAtUtc = eventItem.StartsAtUtc,
                    EndsAtUtc = eventItem.EndsAtUtc,
                    Location = eventItem.Location,
                    SubjectOfferingId = eventItem.SubjectOfferingId!.Value,
                    SubjectName = eventItem.SubjectOffering!.Subject.Name,
                    HasReminder = eventItem.Reminders.Any(reminder => reminder.UserId == request.UserId),
                    ReminderMinutesBefore =eventItem.Reminders
                            .Where(reminder => reminder.UserId == request.UserId)
                            .Select(reminder => (int?)reminder.MinutesBefore)
                            .FirstOrDefault()
                })
            .ToListAsync(cancellationToken);

        var upcomingMeetingsQuery = _context.Meetings.AsNoTracking()
            .Where(meeting =>
                meeting.HostUserId == request.UserId &&
                meeting.ScheduledAt.HasValue &&
                meeting.ScheduledAt.Value >= utcNow);

        var upcomingMeetingsCount = await upcomingMeetingsQuery.CountAsync(cancellationToken);

        var upcomingMeetings = await upcomingMeetingsQuery
                .OrderBy(meeting => meeting.ScheduledAt)
                .ThenBy(meeting => meeting.Id)
                .Take(request.ItemsCount)
                .Select(meeting =>
                    new TeachingStaffHomeMeetingDto
                    {
                        Id = meeting.Id,
                        Title = meeting.Title,
                        ScheduledAtUtc = meeting.ScheduledAt,
                        MinutesDuration = meeting.MinutesDuration,
                        SubjectOfferingId = meeting.SubjectOfferingId,
                        SubjectName = meeting.SubjectOffering.Subject.Name,
                        JoinUrl = meeting.JoinUrl
                    })
                .ToListAsync(cancellationToken);

        var unreadNotificationsCount = await _context.Notifications.AsNoTracking()
                .CountAsync(notification => notification.UserId == request.UserId && !notification.IsRead,cancellationToken);

        var recentNotifications = await _context.Notifications.AsNoTracking()
                .Where(notification => notification.UserId == request.UserId)
                .OrderByDescending(notification => notification.CreatedAt)
                .ThenByDescending(notification => notification.Id)
                .Take(request.ItemsCount)
                .Select(notification =>
                    new TeachingStaffHomeNotificationDto
                    {
                        Id = notification.Id,
                        Type = notification.Type,
                        Title = notification.Title,
                        Body = notification.Body,
                        IsRead = notification.IsRead,
                        CreatedAtUtc = notification.CreatedAt
                    })
                .ToListAsync(cancellationToken);
        var home = new TeachingStaffHomeDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            UpcomingAcademicEventsCount = upcomingAcademicEventsCount,
            UpcomingMeetingsCount = upcomingMeetingsCount,
            UnreadNotificationsCount = unreadNotificationsCount,
            UpcomingEvents = upcomingEvents,
            UpcomingMeetings = upcomingMeetings,
            RecentNotifications = recentNotifications
        };
        return ResultT<TeachingStaffHomeDto>.Success(home);
    }
}