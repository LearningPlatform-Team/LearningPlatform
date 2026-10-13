using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.EventReminders.CreateEventReminder;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.EventReminders
    .SetEventReminder;

public sealed class SetEventReminderCommandHandler : IRequestHandler<SetEventReminderCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public SetEventReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Result> Handle(SetEventReminderCommand request,CancellationToken cancellationToken)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .Where(eventItem => eventItem.Id == request.EventId)
            .Select(eventItem => new
            {
                eventItem.Id,
                eventItem.StartsAtUtc,
                eventItem.SubjectOfferingId,
                eventItem.CreatedByUserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (eventItem is null)
        {
            return Result.Failure("Event was not found.",ErrorType.NotFound);
        }

        var canViewEvent = await CanViewEventAsync(
            eventItem.SubjectOfferingId,
            eventItem.CreatedByUserId,
            request,
            cancellationToken);

        if (!canViewEvent)
        {
            return Result.Failure("You are not allowed to create a reminder for this event.",ErrorType.Failure);
        }
        var utcNow = DateTime.UtcNow;
        if (eventItem.StartsAtUtc <= utcNow)
        {
            return Result.Failure("A reminder cannot be set for an event that has already started.",ErrorType.Failure);
        }

        var reminderTimeUtc = eventItem.StartsAtUtc.AddMinutes(-request.MinutesBefore);

        if (reminderTimeUtc <= utcNow)
        {
            return Result.Failure("The selected reminder time has already passed.",ErrorType.Failure);
        }

        var reminder = await _context.EventReminders
            .FirstOrDefaultAsync(
                reminder =>reminder.EventId == request.EventId &&
                    reminder.UserId == request.UserId,cancellationToken);

        if (reminder is null)
        {
            reminder = new EventReminder
            {
                EventId = request.EventId,
                UserId = request.UserId,
                MinutesBefore = request.MinutesBefore,
                SentAtUtc = null
            };

            await _context.EventReminders.AddAsync(reminder,cancellationToken);
        }
        else
        {
            reminder.MinutesBefore = request.MinutesBefore;

            /*
             * The new reminder time is confirmed to be in the future,
             * so reset the delivery state to allow the background job
             * to send the reminder using the new schedule.
             */
            reminder.SentAtUtc = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private async Task<bool> CanViewEventAsync(Guid? subjectOfferingId,Guid createdByUserId,SetEventReminderCommand request,
        CancellationToken cancellationToken)
    {
        // General events are visible to all authenticated users.
        if (!subjectOfferingId.HasValue)
        {
            return true;
        }

        // Admin can access all events.
        if (request.IsAdmin)
        {
            return true;
        }

        // The event creator can access the event.
        if (createdByUserId == request.UserId)
        {
            return true;
        }

        var hasActiveEnrollment = await _context.Enrollments
            .AsNoTracking()
            .AnyAsync(
                enrollment =>
                    enrollment.StudentId == request.UserId &&
                    enrollment.OfferingId == subjectOfferingId.Value &&
                    enrollment.Status == EnrollmentStatus.Enrolled,
                    cancellationToken);
        return hasActiveEnrollment;
    }
}