using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.CreateSubjectOffering;

public class CreateSubjectOfferingCommandHandler(IApplicationDbContext context)
    : IRequestHandler<CreateSubjectOfferingCommand, Result>
{
    public async Task<Result> Handle(
     CreateSubjectOfferingCommand request,
     CancellationToken cancellationToken)
    {
        var subjectExists = await context.Subjects
            .AnyAsync(s => s.Id == request.SubjectId, cancellationToken);

        if (!subjectExists)
        {
            return Result.Failure(
                "Subject not found.",
                ErrorType.NotFound);
        }

        var termExists = await context.AcademicTerms
            .AnyAsync(t => t.Id == request.TermId, cancellationToken);

        if (!termExists)
        {
            return Result.Failure(
                "Academic term not found.",
                ErrorType.NotFound);
        }

        var doctorExists = await context.DoctorProfiles
            .AnyAsync(d => d.UserId == request.DoctorProfileId, cancellationToken);

        if (!doctorExists)
        {
            return Result.Failure(
                "Doctor profile not found.",
                ErrorType.NotFound);
        }

        var offering = new SubjectOffering
        {
            Id = Guid.NewGuid(),
            SubjectId = request.SubjectId,
            TermId = request.TermId,
            DoctorProfileId = request.DoctorProfileId,
            IsActive = true
        };

        await context.SubjectOfferings.AddAsync(offering, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}