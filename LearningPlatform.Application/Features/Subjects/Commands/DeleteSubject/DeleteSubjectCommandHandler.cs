using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Commands.DeleteSubject;

public class DeleteSubjectCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteSubjectCommand, Result>
{

    public async Task<Result> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject = await context.Subjects
            .FirstOrDefaultAsync(
                s => s.Id == request.Id,
                cancellationToken);

        if (subject is null)
        {
            return Result.Failure(
                "Subject was not found.",
                ErrorType.NotFound);
        }

        var hasRelatedData = await context.Subjects
            .AnyAsync(
                s => s.Id == request.Id &&
                     (s.Offerings.Any() || s.Books.Any()),
                cancellationToken);

        if (hasRelatedData)
        {
            return Result.Failure(
                "Cannot delete this subject because it has related offerings or books.",
                ErrorType.Conflict);
        }

        context.Subjects.Remove(subject);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
