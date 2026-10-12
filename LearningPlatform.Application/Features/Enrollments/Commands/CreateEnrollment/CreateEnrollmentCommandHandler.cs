using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;

public class CreateEnrollmentCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateEnrollmentCommand, Result>
{
    public async Task<Result> Handle(
        CreateEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        var studentExists = await context.StudentProfiles
            .AnyAsync(
                s => s.UserId == request.StudentId,
                cancellationToken);

        if (!studentExists)
        {
            return Result.Failure("Student not found", ErrorType.NotFound);
        }

        var offering = await context.SubjectOfferings
            .FirstOrDefaultAsync(
                o => o.Id == request.OfferingId,
                cancellationToken);

        if (offering is null)
        {
            return Result.Failure("Subject offering not found", ErrorType.NotFound);
        }

        if (!offering.IsActive)
        {
            return Result.Failure("Subject offering is not active", ErrorType.Conflict);
        }

        // 3. Prevent duplicate enrollment.
        var alreadyEnrolled = await context.Enrollments
            .AnyAsync(
                e => e.StudentId == request.StudentId
                  && e.OfferingId == request.OfferingId,
                cancellationToken);

        if (alreadyEnrolled)
        {
            return Result.Failure("Student is already enrolled in this offering", ErrorType.Conflict);
        }

        var enrollment = new Enrollment
        {
            Id = Guid.NewGuid(),
            StudentId = request.StudentId,
            OfferingId = request.OfferingId,
            EnrolledBy = currentUserService.UserId ?? Guid.Empty,
            EnrolledAt = DateTime.UtcNow
        };

        context.Enrollments.Add(enrollment);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}