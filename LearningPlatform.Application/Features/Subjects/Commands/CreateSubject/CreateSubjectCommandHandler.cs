using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Commands.CreateSubject;

public class CreateSubjectCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateSubjectCommand, Result>
{
    public async Task<Result> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        var subjectExists = await context.Subjects
            .AnyAsync(s => s.Code == request.Code || s.Name == request.Name,
            cancellationToken);
        if (subjectExists)
        {
            return Result.Failure("A subject with the same code or name already exists.", ErrorType.Conflict);
        }
        var subject = new Subject
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            YearLevel = request.YearLevel
        };
        context.Subjects.Add(subject);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
