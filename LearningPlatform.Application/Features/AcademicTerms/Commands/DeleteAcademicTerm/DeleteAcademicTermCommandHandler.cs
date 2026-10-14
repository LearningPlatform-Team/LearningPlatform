using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.AcademicTerms.Commands.DeleteAcademicTerm;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class DeleteAcademicTermCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteAcademicTermCommand, Result>
{
    public async Task<Result> Handle(
        DeleteAcademicTermCommand request,
        CancellationToken cancellationToken)
    {
        var academicTerm = await context.AcademicTerms
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (academicTerm is null)
        {
            return Result.Failure("Academic term not found.", ErrorType.NotFound);
        }

        var hasSubjectOfferings = await context.SubjectOfferings
            .AnyAsync(
                x => x.TermId == request.Id,
                cancellationToken);

        if (hasSubjectOfferings)
        {
            return Result.Failure(
                "Cannot delete academic term because it has subject offerings.",
                ErrorType.Conflict);
        }
        context.AcademicTerms.Remove(academicTerm);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}