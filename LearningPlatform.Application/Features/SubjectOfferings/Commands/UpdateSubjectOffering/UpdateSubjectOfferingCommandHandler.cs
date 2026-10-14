using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;

public class UpdateSubjectOfferingCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSubjectOfferingCommand, Result>
{
    public async Task<Result> Handle(
        UpdateSubjectOfferingCommand request,
        CancellationToken cancellationToken)
    {
        var offering = await context.SubjectOfferings
            .FirstOrDefaultAsync(
                o => o.Id == request.Id,
                cancellationToken);

        if (offering is null)
        {
            return Result.Failure(
                "Subject offering not found.",
                ErrorType.NotFound);
        }

        var subjectExists = await context.Subjects
            .AnyAsync(
                s => s.Id == request.SubjectId,
                cancellationToken);

        if (!subjectExists)
        {
            return Result.Failure(
                "Subject not found.",
                ErrorType.NotFound);
        }

        var termExists = await context.AcademicTerms
            .AnyAsync(
                t => t.Id == request.TermId,
                cancellationToken);

        if (!termExists)
        {
            return Result.Failure(
                "Academic term not found.",
                ErrorType.NotFound);
        }

        var doctorExists = await context.DoctorProfiles
            .AnyAsync(
                d => d.UserId == request.DoctorProfileId,
                cancellationToken);

        if (!doctorExists)
        {
            return Result.Failure(
                "Doctor profile not found.",
                ErrorType.NotFound);
        }

        offering.SubjectId = request.SubjectId;
        offering.TermId = request.TermId;
        offering.DoctorProfileId = request.DoctorProfileId;
        offering.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}