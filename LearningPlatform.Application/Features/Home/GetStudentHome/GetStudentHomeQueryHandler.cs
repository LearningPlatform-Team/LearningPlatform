using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Home.GetStudentHome;

public sealed class GetStudentHomeQueryHandler : IRequestHandler<GetStudentHomeQuery,ResultT<StudentHomeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetStudentHomeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<StudentHomeDto>> Handle(GetStudentHomeQuery request,CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var student = await _context.StudentProfiles
            .AsNoTracking()
            .Where(student => student.UserId == request.StudentId)
            .Select(student => new
            {
                StudentId = student.UserId,
                StudentName = student.User.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return ResultT<StudentHomeDto>.Failure("Student was not found.",ErrorType.NotFound);
        }

        var activeOfferingIds = await _context.Enrollments
            .AsNoTracking()
            .Where(enrollment =>
                enrollment.StudentId == request.StudentId &&
                enrollment.Status == EnrollmentStatus.Enrolled)
            .Select(enrollment => enrollment.OfferingId)
            .ToListAsync(cancellationToken);

        var upcomingEventsQuery = _context.Events
            .AsNoTracking()
            .Where(eventItem =>
                eventItem.StartsAtUtc >= utcNow &&
                (
                    eventItem.SubjectOfferingId == null ||
                    (
                        eventItem.SubjectOfferingId.HasValue &&
                        activeOfferingIds.Contains(
                            eventItem.SubjectOfferingId.Value)
                    )
                ));

        var upcomingEventsCount = await upcomingEventsQuery.CountAsync(cancellationToken);

        var upcomingEvents = await upcomingEventsQuery
            .OrderBy(eventItem => eventItem.StartsAtUtc)
            .ThenBy(eventItem => eventItem.Id)
            .Take(request.ItemsCount)
            .Select(eventItem => new StudentHomeEventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Type = eventItem.Type,
                StartsAtUtc = eventItem.StartsAtUtc,
                Location = eventItem.Location,
                SubjectName = eventItem.SubjectOffering != null ? eventItem.SubjectOffering.Subject.Name : null,
                HasReminder = eventItem.Reminders.Any(reminder => reminder.UserId == request.StudentId),
                ReminderMinutesBefore = eventItem.Reminders
                        .Where(reminder => reminder.UserId == request.StudentId)
                        .Select(reminder => (int?)reminder.MinutesBefore)
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var upcomingMeetings = await _context.Meetings
            .AsNoTracking()
            .Where(meeting =>
                activeOfferingIds.Contains(meeting.SubjectOfferingId) &&
                meeting.ScheduledAt.HasValue &&
                meeting.ScheduledAt.Value >= utcNow)
            .OrderBy(meeting => meeting.ScheduledAt)
            .ThenBy(meeting => meeting.Id)
            .Take(request.ItemsCount)
            .Select(meeting => new StudentHomeMeetingDto
            {
                Id = meeting.Id,
                Title = meeting.Title,
                ScheduledAtUtc = meeting.ScheduledAt,
                MinutesDuration = meeting.MinutesDuration,
                SubjectName = meeting.SubjectOffering.Subject.Name,
                JoinUrl = meeting.JoinUrl
            })
            .ToListAsync(cancellationToken);

        var unreadNotificationsCount =
            await _context.Notifications
                .AsNoTracking()
                .CountAsync(notification => notification.UserId == request.StudentId && !notification.IsRead,cancellationToken);

        var recentNotifications =
            await _context.Notifications
                .AsNoTracking()
                .Where(notification => notification.UserId == request.StudentId)
                .OrderByDescending(notification => notification.CreatedAt)
                .ThenBy(notification => notification.Id)
                .Take(request.ItemsCount)
                .Select(notification =>
                    new StudentHomeNotificationDto
                    {
                        Id = notification.Id,
                        Type = notification.Type,
                        Title = notification.Title,
                        Body = notification.Body,
                        IsRead = notification.IsRead,
                        CreatedAtUtc = notification.CreatedAt
                    })
                .ToListAsync(cancellationToken);

        var attendanceCounts = await _context.AttendanceRecords
                .AsNoTracking()
                .Where(attendance => attendance.Enrollment.StudentId == request.StudentId)
                .GroupBy(_ => 1)
                .Select(group => new
                {
                    Total = group.Count(),
                    Attended = group.Count(attendance => attendance.Status == AttendanceStatus.Present ||
                        attendance.Status == AttendanceStatus.Late)
                })
                .FirstOrDefaultAsync(cancellationToken);

        var attendancePercentage = attendanceCounts is null || attendanceCounts.Total == 0
                ? 0
                : Math.Round(attendanceCounts.Attended * 100m /attendanceCounts.Total,2);

        var home = new StudentHomeDto
        {
            StudentId = student.StudentId,
            StudentName = student.StudentName,
            ActiveEnrollmentsCount = activeOfferingIds.Count,
            UnreadNotificationsCount = unreadNotificationsCount,
            UpcomingEventsCount = upcomingEventsCount,
            AttendancePercentage = attendancePercentage,
            UpcomingEvents = upcomingEvents,
            UpcomingMeetings = upcomingMeetings,
            RecentNotifications = recentNotifications
        };

        return ResultT<StudentHomeDto>.Success(home);
    }
}