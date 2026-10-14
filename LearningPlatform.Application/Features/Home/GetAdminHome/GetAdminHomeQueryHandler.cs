using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Home.GetAdminHome;

public sealed class GetAdminHomeQueryHandler : IRequestHandler<GetAdminHomeQuery,ResultT<AdminHomeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAdminHomeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<AdminHomeDto>> Handle(GetAdminHomeQuery request,CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var admin = await _context.Users.AsNoTracking()
            .Where(user => user.Id == request.AdminId)
            .Select(user => new
            {
                user.Id,
                user.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (admin is null)
        {
            return ResultT<AdminHomeDto>.Failure("Admin user was not found.",ErrorType.NotFound);
        }

        var userStatistics = await _context.Users.AsNoTracking()
            .GroupBy(_ => 1)
            .Select(group => new
            {
                TotalUsersCount = group.Count(),
                TotalStudentsCount = group.Count(user => user.UserRoles.Any(userRole => userRole.Role.Name == "Student")),

                TotalDoctorsCount = group.Count(user => user.UserRoles.Any(userRole => userRole.Role.Name == "Doctor")),

                TotalTeachingAssistantsCount = group.Count(user => user.UserRoles.Any(userRole => userRole.Role.Name == "TA")),
                TotalAdminsCount = group.Count(user => user.UserRoles.Any(userRole => userRole.Role.Name == "Admin"))
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalUsersCount = userStatistics?.TotalUsersCount ?? 0;
        var totalStudentsCount = userStatistics?.TotalStudentsCount ?? 0;
        var totalDoctorsCount = userStatistics?.TotalDoctorsCount ?? 0;
        var totalTeachingAssistantsCount = userStatistics?.TotalTeachingAssistantsCount ?? 0;
        var totalAdminsCount = userStatistics?.TotalAdminsCount ?? 0;
        var totalSubjectOfferingsCount = await _context.SubjectOfferings.AsNoTracking().CountAsync(cancellationToken);
        var upcomingEventsCount = await _context.Events.AsNoTracking()
            .CountAsync(eventItem => eventItem.StartsAtUtc >= utcNow,cancellationToken);
        var upcomingGeneralEvents = await _context.Events.AsNoTracking()
                .Where(eventItem => eventItem.SubjectOfferingId == null &&eventItem.StartsAtUtc >= utcNow)
                .OrderBy(eventItem => eventItem.StartsAtUtc)
                .ThenBy(eventItem => eventItem.Id)
                .Take(request.ItemsCount)
                .Select(eventItem =>
                    new AdminHomeEventDto
                    {
                        Id = eventItem.Id,
                        Title = eventItem.Title,
                        Type = eventItem.Type,
                        StartsAtUtc = eventItem.StartsAtUtc,
                        EndsAtUtc = eventItem.EndsAtUtc,
                        Location = eventItem.Location,
                        CreatedByUserId = eventItem.CreatedByUserId,
                        CreatedByUserName = eventItem.CreatedByUser.FullName,
                        HasReminder = eventItem.Reminders.Any( reminder => reminder.UserId == request.AdminId),
                        ReminderMinutesBefore = eventItem.Reminders
                                .Where(reminder => reminder.UserId == request.AdminId)
                                .Select(reminder => (int?)reminder.MinutesBefore)
                                .FirstOrDefault()
                    })
                .ToListAsync(cancellationToken);

        var upcomingMeetingsQuery = _context.Meetings.AsNoTracking()
            .Where(meeting =>
                meeting.ScheduledAt.HasValue && meeting.ScheduledAt.Value >= utcNow);

        var upcomingMeetingsCount = await upcomingMeetingsQuery.CountAsync(cancellationToken);
        var upcomingMeetings = await upcomingMeetingsQuery
                .OrderBy(meeting => meeting.ScheduledAt)
                .ThenBy(meeting => meeting.Id)
                .Take(request.ItemsCount)
                .Select(meeting =>
                    new AdminHomeMeetingDto
                    {
                        Id = meeting.Id,
                        Title = meeting.Title,
                        ScheduledAtUtc = meeting.ScheduledAt,
                        MinutesDuration = meeting.MinutesDuration,
                        SubjectOfferingId = meeting.SubjectOfferingId,
                        SubjectName = meeting.SubjectOffering.Subject.Name,
                        HostName = meeting.HostUser.FullName
                    })
                .ToListAsync(cancellationToken);

        var unreadNotificationsCount =
            await _context.Notifications.AsNoTracking()
                .CountAsync(notification => notification.UserId == request.AdminId && !notification.IsRead, cancellationToken);
        var recentNotifications = await _context.Notifications.AsNoTracking()
                .Where(notification => notification.UserId == request.AdminId)
                .OrderByDescending(notification => notification.CreatedAt)
                .ThenByDescending(notification => notification.Id)
                .Take(request.ItemsCount)
                .Select(notification =>
                    new AdminHomeNotificationDto
                    {
                        Id = notification.Id,
                        Type = notification.Type,
                        Title = notification.Title,
                        Body = notification.Body,
                        IsRead = notification.IsRead,
                        CreatedAtUtc = notification.CreatedAt
                    })
                .ToListAsync(cancellationToken);

        var home = new AdminHomeDto
        {
            AdminId = admin.Id,
            AdminName = admin.FullName,
            TotalUsersCount = totalUsersCount,
            TotalStudentsCount = totalStudentsCount,
            TotalDoctorsCount = totalDoctorsCount,
            TotalTeachingAssistantsCount = totalTeachingAssistantsCount,
            TotalAdminsCount = totalAdminsCount,
            TotalSubjectOfferingsCount = totalSubjectOfferingsCount,
            UpcomingEventsCount = upcomingEventsCount,
            UpcomingMeetingsCount = upcomingMeetingsCount,
            UnreadNotificationsCount = unreadNotificationsCount,
            UpcomingGeneralEvents = upcomingGeneralEvents,
            UpcomingMeetings = upcomingMeetings,
            RecentNotifications = recentNotifications
        };
        return ResultT<AdminHomeDto>.Success(home);
    }
}