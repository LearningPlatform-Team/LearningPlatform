using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainEvent = LearningPlatform.Domain.Entities.Engagement.Event;

namespace LearningPlatform.Application.Features.Events.CreateEvent;

public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, ResultT<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateEventCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ResultT<Guid>> Handle(CreateEventCommand request,CancellationToken cancellationToken)
    {
        var creatorExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == request.CreatedByUserId,cancellationToken);

        if (!creatorExists)
        {
            return ResultT<Guid>.Failure("The user creating the event was not found.",
                ErrorType.NotFound);
        }

        var isAcademicEvent = request.SubjectOfferingId.HasValue;
        var isGeneralEvent = !isAcademicEvent;

        if (isGeneralEvent && !request.IsAdmin)
        {
            return ResultT<Guid>.Failure("Only administrators can create general events.",
                ErrorType.Failure);
        }

        if (isAcademicEvent && !request.IsDoctor && !request.IsTA)
        {
            return ResultT<Guid>.Failure("Only doctors and teaching assistants can create academic events.",
                ErrorType.Failure);
        }

        if (isAcademicEvent)
        {
            var subjectOfferingExists = await _context.SubjectOfferings
                    .AsNoTracking()
                    .AnyAsync( offering => offering.Id == request.SubjectOfferingId!.Value,cancellationToken);

            if (!subjectOfferingExists)
            {
                return ResultT<Guid>.Failure("Subject offering was not found.",
                    ErrorType.NotFound);
            }
        }

        var eventItem = new DomainEvent
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Type = request.Type,
            StartsAtUtc = request.StartsAtUtc,
            EndsAtUtc = request.EndsAtUtc,
            Location = string.IsNullOrWhiteSpace(request.Location) ? null : request.Location.Trim(),
            SubjectOfferingId = request.SubjectOfferingId,
            CreatedByUserId = request.CreatedByUserId
        };

        await _context.Events.AddAsync(eventItem,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return ResultT<Guid>.Success(eventItem.Id);
    }
}