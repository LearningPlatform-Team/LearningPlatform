using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Events.UpdateEvent;

public sealed class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateEventCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var eventItem = await _context.Events.FirstOrDefaultAsync(eventItem => eventItem.Id == request.EventId,
                cancellationToken);

        if (eventItem is null)
        {
            return Result.Failure("Event was not found.",
                ErrorType.NotFound);
        }

        var isAcademicEvent = eventItem.SubjectOfferingId.HasValue;
        var isGeneralEvent = !isAcademicEvent;

        if (isGeneralEvent && !request.IsAdmin)
        {
            return Result.Failure("Only administrators can update general events.",
                ErrorType.Failure);
        }

        if (isAcademicEvent && !request.IsDoctor && !request.IsTA)
        {
            return Result.Failure("Only doctors and teaching assistants can update academic events.",
                ErrorType.Failure);
        }

        if (eventItem.CreatedByUserId != request.UpdatedByUserId)
        {
            return Result.Failure("You are not allowed to update an event created by another user.",
                ErrorType.Failure);
        }

        if (isGeneralEvent && request.SubjectOfferingId.HasValue)
        {
            return Result.Failure("A general event cannot be converted into an academic event.",
                ErrorType.Failure);
        }

        if (isAcademicEvent && !request.SubjectOfferingId.HasValue)
        {
            return Result.Failure("An academic event must remain linked to a subject offering.",
                ErrorType.Failure);
        }

        if (isAcademicEvent)
        {
            var subjectOfferingExists =
                await _context.SubjectOfferings
                    .AsNoTracking()
                    .AnyAsync(offering => offering.Id == request.SubjectOfferingId!.Value,cancellationToken);

            if (!subjectOfferingExists)
            {
                return Result.Failure("Subject offering was not found.",
                    ErrorType.NotFound);
            }
        }

        eventItem.Title = request.Title.Trim();
        eventItem.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        eventItem.Type = request.Type;
        eventItem.StartsAtUtc = request.StartsAtUtc;
        eventItem.EndsAtUtc = request.EndsAtUtc;
        eventItem.Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim();
        eventItem.SubjectOfferingId = request.SubjectOfferingId;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}