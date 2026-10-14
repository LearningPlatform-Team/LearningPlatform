using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;

public class CreateAcademicTermCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CreateAcademicTermCommand, Result>
{
    public async Task<Result> Handle(
        CreateAcademicTermCommand request,
        CancellationToken cancellationToken)
    {
        var termExists = await context.AcademicTerms
            .AnyAsync(
                t => t.AcademicYear == request.AcademicYear &&
                     t.TermNumber == request.TermNumber,
                cancellationToken);

        if (termExists)
        {
            return Result.Failure(
                "This term already exists for the academic year.",
                ErrorType.Conflict);
        }

        var term = new AcademicTerm
        {
            Id = Guid.NewGuid(),
            AcademicYear = request.AcademicYear,
            TermNumber = request.TermNumber,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsCurrent = false
        };

        await context.AcademicTerms.AddAsync(term, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}