using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.AcademicTerms.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.AcademicTerms.Queries.GetAcademicTermById;

public class GetAcademicTermByIdQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAcademicTermByIdQuery, ResultT<AcademicTermResponse>>
{
    public async Task<ResultT<AcademicTermResponse>> Handle(
        GetAcademicTermByIdQuery request,
        CancellationToken cancellationToken)
    {
        var term = await context.AcademicTerms
            .AsNoTracking()
            .Where(t => t.Id == request.Id)
            .Select(t => new AcademicTermResponse(
                t.Id,
                t.AcademicYear,
                t.TermNumber,
                t.StartDate,
                t.EndDate,
                t.IsCurrent))
            .FirstOrDefaultAsync(cancellationToken);

        if (term is null)
        {
            return ResultT<AcademicTermResponse>.Failure(
                "Academic term not found.",
                ErrorType.NotFound);
        }

        return ResultT<AcademicTermResponse>.Success(term);
    }
}