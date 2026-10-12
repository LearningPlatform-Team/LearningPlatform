using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.EventReminders.DeleteEventReminder;

public sealed class DeleteEventReminderCommandHandler : IRequestHandler<DeleteEventReminderCommand, Result>
{
    private readonly IApplicationDbContext _context;
    public DeleteEventReminderCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteEventReminderCommand request,CancellationToken cancellationToken)
    {
        var reminder = await _context.EventReminders
            .FirstOrDefaultAsync(reminder => reminder.EventId == request.EventId &&
                    reminder.UserId == request.UserId,cancellationToken);

        if (reminder is null)
        {
            return Result.Failure("Event reminder was not found.",ErrorType.NotFound);
        }
        _context.EventReminders.Remove(reminder);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}