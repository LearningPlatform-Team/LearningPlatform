using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Commands.CancelEnrollment;

public class CancelEnrollmentCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CancelEnrollmentCommand, Result>
{
    public async Task<Result> Handle(
        CancelEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        var enrollment = await context.Enrollments
            .FirstOrDefaultAsync(
                e => e.Id == request.EnrollmentId,
                cancellationToken);

        if (enrollment is null)
            return Result.Failure("Enrollment not found", ErrorType.NotFound);

        if (enrollment.Status == EnrollmentStatus.Dropped)
            return Result.Failure("Enrollment is already dropped", ErrorType.Validation);

        if (enrollment.Status == EnrollmentStatus.Completed)
            return Result.Failure("Cannot drop completed enrollment", ErrorType.Validation);

        enrollment.Status = EnrollmentStatus.Dropped;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}