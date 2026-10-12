using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;

public class GetMyEnrollmentHistoryQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<
        GetMyEnrollmentHistoryQuery,
        ResultT<List<EnrollmentHistoryResponse>>>
{
    public async Task<ResultT<List<EnrollmentHistoryResponse>>> Handle(
        GetMyEnrollmentHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var history = await context.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.Student.UserId == userId &&
                e.Status == EnrollmentStatus.Completed)
            .OrderByDescending(e => e.EnrolledAt)
            .Select(e => new EnrollmentHistoryResponse(
                e.Id,
                e.OfferingId,
                e.SubjectOffering.Subject.Code,
                e.SubjectOffering.Subject.Name,
                e.Status,
                e.EnrolledAt))
            .ToListAsync(cancellationToken);

        return ResultT<List<EnrollmentHistoryResponse>>.Success(history);
    }
}