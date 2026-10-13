using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEvent = LearningPlatform.Domain.Entities.Engagement.Event;

namespace LearningPlatform.Application.Features.Events.GetUpcomingEvents;

public sealed class GetUpcomingEventsQueryHandler : IRequestHandler<GetUpcomingEventsQuery, ResultT<IReadOnlyList<UpcomingEventDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetUpcomingEventsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<IReadOnlyList<UpcomingEventDto>>> Handle(GetUpcomingEventsQuery request,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var eventsQuery = _context.Events
            .AsNoTracking()
            .Where(eventItem => eventItem.StartsAtUtc >= utcNow);

        eventsQuery = ApplyVisibilityFilter(eventsQuery,request);

        var upcomingEvents = await eventsQuery
            .OrderBy(eventItem => eventItem.StartsAtUtc)
            .ThenBy(eventItem => eventItem.Id)
            .Take(request.Take)
            .Select(eventItem => new UpcomingEventDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Type = eventItem.Type,
                StartsAtUtc = eventItem.StartsAtUtc,
                EndsAtUtc = eventItem.EndsAtUtc,
                Location = eventItem.Location,
                IsGeneral = eventItem.SubjectOfferingId == null,
                SubjectOfferingId = eventItem.SubjectOfferingId,
                SubjectName =
                    eventItem.SubjectOffering != null ? eventItem.SubjectOffering.Subject.Name : null,
                HasReminder =
                    eventItem.Reminders
                        .Any(reminder => reminder.UserId == request.RequestedByUserId),
                ReminderMinutesBefore = eventItem.Reminders
                        .Where(reminder => reminder.UserId == request.RequestedByUserId)
                        .Select(reminder => (int?)reminder.MinutesBefore)
                        .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return ResultT<IReadOnlyList<UpcomingEventDto>>.Success(upcomingEvents);
    }

    private IQueryable<DomainEvent> ApplyVisibilityFilter(IQueryable<DomainEvent> eventsQuery, GetUpcomingEventsQuery request)
    {
        if (request.IsAdmin)
        {
            return eventsQuery;
        }

        /*
         * Doctors and TAs currently see:
         * - General events.
         * - Academic events they created.
         *
         * Offering-assignment filtering can be added after
         * confirming the exact Doctor/TA relationship.
         */
        if (request.IsDoctor || request.IsTA)
        {
            return eventsQuery.Where(eventItem => eventItem.SubjectOfferingId == null ||
                eventItem.CreatedByUserId == request.RequestedByUserId);
        }

        /*
         * Students see:
         * - General events.
         * - Academic events for subject offerings in which
         *   they have an active enrollment.
         */
        return eventsQuery.Where(eventItem => eventItem.SubjectOfferingId == null || _context.Enrollments.Any(
            enrollment =>
                enrollment.StudentId == request.RequestedByUserId &&
                enrollment.OfferingId == eventItem.SubjectOfferingId &&
                enrollment.Status == EnrollmentStatus.Enrolled));
    }
}