using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollmentById;

public class GetEnrollmentByIdQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetEnrollmentByIdQuery, ResultT<EnrollmentResponse>>
{
    public async Task<ResultT<EnrollmentResponse>> Handle(
        GetEnrollmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var enrollment = await context.Enrollments
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new EnrollmentResponse(
                e.Id,
                e.StudentId,
                e.OfferingId,
                e.EnrolledBy,
                e.EnrolledAt,
                e.Status))
            .FirstOrDefaultAsync(cancellationToken);

        if (enrollment is null)
        {
            // Replace with your project's existing NotFound result pattern.
            return ResultT<EnrollmentResponse>.Failure(
                "Enrollment not found.");
        }

        return ResultT<EnrollmentResponse>.Success(enrollment);
    }
}