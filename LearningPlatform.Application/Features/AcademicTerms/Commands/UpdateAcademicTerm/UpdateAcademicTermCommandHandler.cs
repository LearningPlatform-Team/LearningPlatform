using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.UpdateAcademicTerm;

public class UpdateAcademicTermCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<UpdateAcademicTermCommand, Result>
{
    public async Task<Result> Handle(
        UpdateAcademicTermCommand request,
        CancellationToken cancellationToken)
    {
        var term = await context.AcademicTerms
            .FirstOrDefaultAsync(
                t => t.Id == request.Id,
                cancellationToken);

        if (term is null)
        {
            return Result.Failure(
                "Academic term not found.",
                ErrorType.NotFound);
        }

        var duplicateExists = await context.AcademicTerms
            .AnyAsync(
                t => t.Id != request.Id &&
                     t.AcademicYear == request.AcademicYear &&
                     t.TermNumber == request.TermNumber,
                cancellationToken);

        if (duplicateExists)
        {
            return Result.Failure(
                "This term already exists for the academic year.",
                ErrorType.Conflict);
        }

        term.AcademicYear = request.AcademicYear;
        term.TermNumber = request.TermNumber;
        term.StartDate = request.StartDate;
        term.EndDate = request.EndDate;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}