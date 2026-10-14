using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Events.GetEventById;

public sealed class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, ResultT<EventDetailsDto>>
{
    private readonly IApplicationDbContext _context;
    public GetEventByIdQueryHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<EventDetailsDto>> Handle(GetEventByIdQuery request,CancellationToken cancellationToken)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .Where(eventItem => eventItem.Id == request.EventId)
            .Select(eventItem => new EventDetailsDto
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
                HasReminder = eventItem.Reminders.Any( reminder => reminder.UserId == request.RequestedByUserId),
                ReminderMinutesBefore =eventItem.Reminders
                        .Where(reminder => reminder.UserId == request.RequestedByUserId)
                        .Select(reminder => (int?)reminder.MinutesBefore)
                        .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (eventItem is null)
        {
            return ResultT<EventDetailsDto>.Failure("Event was not found.",
                ErrorType.NotFound);
        }

        var canViewEvent = await CanViewEventAsync(
            eventItem,
            request,
            cancellationToken);

        if (!canViewEvent)
        {
            return ResultT<EventDetailsDto>.Failure("You are not allowed to view this event.",
                ErrorType.Failure);
        }

        return ResultT<EventDetailsDto>.Success(eventItem);
    }

    private async Task<bool> CanViewEventAsync(
        EventDetailsDto eventItem,
        GetEventByIdQuery request,
        CancellationToken cancellationToken)
    {
        // General events are visible to every authenticated user.
        if (eventItem.IsGeneral)
        {
            return true;
        }

        // Admin can view any academic or general event.
        if (request.IsAdmin)
        {
            return true;
        }

        // The event creator can always view the event.
        if (eventItem.CreatedByUserId ==
            request.RequestedByUserId)
        {
            return true;
        }

        if (!eventItem.SubjectOfferingId.HasValue)
        {
            return false;
        }

        /*
         * Students can view an academic event when they have
         * an active enrollment in its subject offering.
         */
        var hasActiveEnrollment =
            await _context.Enrollments
                .AsNoTracking()
                .AnyAsync(enrollment =>
                        enrollment.StudentId == request.RequestedByUserId &&
                        enrollment.OfferingId == eventItem.SubjectOfferingId.Value &&
                        enrollment.Status == EnrollmentStatus.Enrolled,
                        cancellationToken);
        if (hasActiveEnrollment)
        {
            return true;
        }

        /*
         * A Doctor or TA who did not create the event will need
         * a subject-offering assignment check.
         *
         * That check should be added after confirming the actual
         * relationship between Doctor/TA and SubjectOffering.
         */
        if (request.IsDoctor || request.IsTA)
        {
            return false;
        }
        return false;
    }
}