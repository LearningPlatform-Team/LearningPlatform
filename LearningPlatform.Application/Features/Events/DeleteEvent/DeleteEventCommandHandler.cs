using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Events.DeleteEvent;

public sealed class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeleteEventCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var eventItem = await _context.Events
            .FirstOrDefaultAsync(eventItem => eventItem.Id == request.EventId, cancellationToken);
        if (eventItem is null)
        {
            return Result.Failure("Event was not found.",
                ErrorType.NotFound);
        }

        var isAcademicEvent = eventItem.SubjectOfferingId.HasValue;
        var isGeneralEvent = !isAcademicEvent;
        if (isGeneralEvent && !request.IsAdmin)
        {
            return Result.Failure("Only administrators can delete general events.",
                ErrorType.Failure);
        }

        if (isAcademicEvent && !request.IsDoctor && !request.IsTA)
        {
            return Result.Failure("Only doctors and teaching assistants can delete academic events.",
                ErrorType.Failure);
        }

        if (eventItem.CreatedByUserId != request.DeletedByUserId)
        {
            return Result.Failure("You are not allowed to delete an event created by another user.",
                ErrorType.Failure);
        }
        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync(
            cancellationToken);
        return Result.Success();
    }
}