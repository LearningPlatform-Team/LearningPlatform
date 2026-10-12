using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.EventReminders.GetEventReminders;

public sealed class GetEventRemindersQueryHandler : IRequestHandler<GetEventRemindersQuery,ResultT<PaginatedList<EventReminderDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetEventRemindersQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<ResultT<PaginatedList<EventReminderDto>>> Handle(GetEventRemindersQuery request,CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;
        var remindersQuery = _context.EventReminders
            .AsNoTracking()
            .Where(reminder => reminder.UserId == request.UserId);

        if (request.UpcomingOnly)
        {
            remindersQuery = remindersQuery.Where(reminder => reminder.Event.StartsAtUtc > utcNow);
        }

        var projectedQuery = remindersQuery
            .Select(reminder => new EventReminderDto
            {
                EventId = reminder.EventId,
                EventTitle = reminder.Event.Title,
                EventType = reminder.Event.Type,
                StartsAtUtc = reminder.Event.StartsAtUtc,
                EndsAtUtc = reminder.Event.EndsAtUtc,
                Location = reminder.Event.Location,
                MinutesBefore = reminder.MinutesBefore,
                ReminderAtUtc = reminder.Event.StartsAtUtc.AddMinutes(-reminder.MinutesBefore),
                IsSent = reminder.SentAtUtc.HasValue,
                SentAtUtc = reminder.SentAtUtc,
                IsGeneral = reminder.Event.SubjectOfferingId == null,
                SubjectOfferingId = reminder.Event.SubjectOfferingId,
                SubjectName = reminder.Event.SubjectOffering != null ? reminder.Event.SubjectOffering.Subject.Name : null
            });

        var totalCount = await projectedQuery.CountAsync(cancellationToken);

        var items = await projectedQuery
            .OrderBy(reminder => reminder.ReminderAtUtc)
            .ThenBy(reminder => reminder.EventId)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paginatedReminders = new PaginatedList<EventReminderDto>(items,totalCount,request.PageNumber,request.PageSize);
        return ResultT<PaginatedList<EventReminderDto>>.Success(paginatedReminders);
    }
}