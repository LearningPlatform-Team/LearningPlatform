
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models;
using LearningPlatform.Application.Features.SubjectOfferings.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;

public class GetSubjectOfferingsHandler(
    IApplicationDbContext context)
    : IRequestHandler<
        GetSubjectOfferingsQuery,
        ResultT<PagedResult<SubjectOfferingResponse>>>
{
    public async Task<ResultT<PagedResult<SubjectOfferingResponse>>> Handle(
        GetSubjectOfferingsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.SubjectOfferings
            .AsNoTracking()
            .AsQueryable();

        if (request.TermId.HasValue)
        {
            query = query.Where(x => x.TermId == request.TermId.Value);
        }

        if (request.SubjectId.HasValue)
        {
            query = query.Where(x => x.SubjectId == request.SubjectId.Value);
        }

        if (request.DoctorProfileId.HasValue)
        {
            query = query.Where(
                x => x.DoctorProfileId == request.DoctorProfileId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderBy(x => x.Subject.Name)
            .ThenBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SubjectOfferingResponse(
                x.Id,
                x.SubjectId,
                x.Subject.Code,
                x.Subject.Name,
                x.Subject.YearLevel,
                x.TermId,
                x.DoctorProfileId,
                x.IsActive
            ))
            .ToListAsync(cancellationToken);

        return ResultT<PagedResult<SubjectOfferingResponse>>.Success(
            new PagedResult<SubjectOfferingResponse>(
                items,
                totalCount,
                pageNumber,
                pageSize));
    }
}
