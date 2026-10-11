using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;

public class GetSubjectByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSubjectByIdQuery, ResultT<SubjectResponse>>
{
    public async Task<ResultT<SubjectResponse>> Handle(
        GetSubjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var subject = await context.Subjects
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .Select(s => new SubjectResponse(
                s.Id,
                s.Code,
                s.Name,
                s.Description,
                s.YearLevel.ToString()))
            .FirstOrDefaultAsync(cancellationToken);

        if (subject is null)
        {
            return ResultT<SubjectResponse>.Failure(
                "Subject not found.",
                ErrorType.NotFound);
        }

        return ResultT<SubjectResponse>.Success(subject);
    }
}