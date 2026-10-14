using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Queries.GetLectureById;

public sealed class GetLectureByIdQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetLectureByIdQuery, ResultT<LectureResponse>>
{
    public async Task<ResultT<LectureResponse>> Handle(
        GetLectureByIdQuery request,
        CancellationToken cancellationToken)
    {
        var lecture = await context.Lectures
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new LectureResponse(
                x.Id,
                x.SubjectOfferingId,
                x.UploadedById,
                x.Title,
                x.Description,
                x.DurationMinutes,
                x.VideoProvider,
                x.VideoUrl,
                x.SortOrder,
                x.CreatedAt,
                x.IsPublished))
            .FirstOrDefaultAsync(cancellationToken);

        if (lecture is null)
        {
            return ResultT<LectureResponse>.Failure(
                "Lecture was not found.",
                ErrorType.NotFound);
        }

        return ResultT<LectureResponse>.Success(lecture);
    }
}