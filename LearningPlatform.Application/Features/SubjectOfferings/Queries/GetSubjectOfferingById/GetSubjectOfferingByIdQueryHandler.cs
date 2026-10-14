using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.SubjectOfferings.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferingById;

public sealed class GetSubjectOfferingByIdQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetSubjectOfferingByIdQuery, ResultT<SubjectOfferingResponse>>
{
    public async Task<ResultT<SubjectOfferingResponse>> Handle(
        GetSubjectOfferingByIdQuery request,
        CancellationToken cancellationToken)
    {
        var offering = await context.SubjectOfferings
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (offering is null)
        {
            return ResultT<SubjectOfferingResponse>.Failure(
                "Subject offering was not found.",
                ErrorType.NotFound);
        }

        return ResultT<SubjectOfferingResponse>.Success(offering);
    }
}