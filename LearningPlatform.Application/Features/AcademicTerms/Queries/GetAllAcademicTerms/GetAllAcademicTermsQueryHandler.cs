using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.AcademicTerms.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.AcademicTerms.Queries.GetAllAcademicTerms;

public class GetAllAcademicTermsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllAcademicTermsQuery, ResultT<List<AcademicTermResponse>>>
{
    public async Task<ResultT<List<AcademicTermResponse>>> Handle(
        GetAllAcademicTermsQuery request,
        CancellationToken cancellationToken)
    {
        var terms = await context.AcademicTerms
            .AsNoTracking()
            .OrderByDescending(t => t.AcademicYear)
            .ThenBy(t => t.TermNumber)
            .Select(t => new AcademicTermResponse(
                t.Id,
                t.AcademicYear,
                t.TermNumber,
                t.StartDate,
                t.EndDate,
                t.IsCurrent))
            .ToListAsync(cancellationToken);

        return ResultT<List<AcademicTermResponse>>.Success(terms);
    }
}