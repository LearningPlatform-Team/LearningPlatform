using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;

public class GetEnrollmentsQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetEnrollmentsQuery, ResultT<PagedResult<EnrollmentResponse>>>
{
    public async Task<ResultT<PagedResult<EnrollmentResponse>>> Handle(
        GetEnrollmentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Enrollments
            .AsNoTracking()
            .AsQueryable();

        if (request.StudentId.HasValue)
        {
            query = query.Where(e =>
                e.StudentId == request.StudentId.Value);
        }

        if (request.OfferingId.HasValue)
        {
            query = query.Where(e =>
                e.OfferingId == request.OfferingId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(e =>
                e.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var enrollments = await query
            .OrderByDescending(e => e.EnrolledAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EnrollmentResponse(
                e.Id,
                e.StudentId,
                e.OfferingId,
                e.EnrolledBy,
                e.EnrolledAt,
                e.Status))
            .ToListAsync(cancellationToken);

        var response = new PagedResult<EnrollmentResponse>(
            enrollments,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return ResultT<PagedResult<EnrollmentResponse>>.Success(response);
    }
}