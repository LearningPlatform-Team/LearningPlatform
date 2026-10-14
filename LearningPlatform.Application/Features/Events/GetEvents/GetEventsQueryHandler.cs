using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LearningPlatform.Domain.Entities.Engagement;
namespace LearningPlatform.Application.Features.Events.GetEvents;

public sealed class GetEventsQueryHandler : IRequestHandler<GetEventsQuery,ResultT<PaginatedList<EventListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEventsQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<PaginatedList<EventListItemDto>>> Handle(GetEventsQuery request,CancellationToken cancellationToken)
    {
        var eventsQuery = _context.Events
            .AsNoTracking()
            .AsQueryable();

        eventsQuery = ApplyVisibilityFilter(eventsQuery,request);

        if (request.Type.HasValue)
        {
            eventsQuery = eventsQuery.Where(eventItem =>eventItem.Type == request.Type.Value);
        }

        if (request.SubjectOfferingId.HasValue)
        {
            eventsQuery = eventsQuery
                .Where(eventItem => eventItem.SubjectOfferingId == request.SubjectOfferingId.Value);
        }

        if (request.FromUtc.HasValue)
        {
            eventsQuery = eventsQuery
                .Where(eventItem => eventItem.StartsAtUtc >= request.FromUtc.Value);
        }

        if (request.ToUtc.HasValue)
        {
            eventsQuery = eventsQuery
                .Where(eventItem => eventItem.StartsAtUtc <= request.ToUtc.Value);
        }

        if (request.UpcomingOnly)
        {
            var utcNow = DateTime.UtcNow;
            eventsQuery = eventsQuery.Where(eventItem => eventItem.StartsAtUtc >= utcNow);
        }

        var projectedQuery = eventsQuery
            .Select(eventItem => new EventListItemDto
            {
                Id = eventItem.Id,
                Title = eventItem.Title,
                Description = eventItem.Description,
                Type = eventItem.Type,
                StartsAtUtc = eventItem.StartsAtUtc,
                EndsAtUtc = eventItem.EndsAtUtc,
                Location = eventItem.Location,
                IsGeneral = eventItem.SubjectOfferingId == null,
                SubjectOfferingId = eventItem.SubjectOfferingId,
                SubjectName = eventItem.SubjectOffering != null ? eventItem.SubjectOffering.Subject.Name : null,
                CreatedByUserId = eventItem.CreatedByUserId,
                CreatedByUserName = eventItem.CreatedByUser.FullName,
                HasReminder = eventItem.Reminders
                    .Any(reminder => reminder.UserId == request.RequestedByUserId),
                ReminderMinutesBefore = eventItem.Reminders
                        .Where(reminder => reminder.UserId == request.RequestedByUserId)
                        .Select(reminder => (int?)reminder.MinutesBefore)
                        .FirstOrDefault()
            });

        var totalCount = await projectedQuery.CountAsync(
            cancellationToken);

        var items = await projectedQuery
            .OrderBy(eventItem => eventItem.StartsAtUtc)
            .ThenBy(eventItem => eventItem.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paginatedEvents =
            new PaginatedList<EventListItemDto>(
                items,
                totalCount,
                request.PageNumber,
                request.PageSize);

        return ResultT<PaginatedList<EventListItemDto>>.Success(paginatedEvents);
    }

    private IQueryable<Event> ApplyVisibilityFilter(IQueryable<Event>eventsQuery,GetEventsQuery request)
    {
        // Admin can view every event.
        if (request.IsAdmin)
        {
            return eventsQuery;
        }

        /*
         * Doctors and TAs currently view:
         * - All general events.
         * - Academic events they created.
         *
         * Offering-assignment visibility can be added later
         * when the exact Doctor/TA relationship is confirmed.
         */
        if (request.IsDoctor || request.IsTA)
        {
            return eventsQuery.Where(eventItem => eventItem.SubjectOfferingId == null ||
                eventItem.CreatedByUserId == request.RequestedByUserId);
        }

        /*
         * Students view:
         * - All general events.
         * - Events belonging to offerings in which they
         *   have an active enrollment.
         */
        return eventsQuery.Where(eventItem => eventItem.SubjectOfferingId == null || _context.Enrollments.Any(
                    enrollment =>
                        enrollment.StudentId == request.RequestedByUserId &&
                        enrollment.OfferingId == eventItem.SubjectOfferingId &&
                        enrollment.Status == EnrollmentStatus.Enrolled));
    }
}