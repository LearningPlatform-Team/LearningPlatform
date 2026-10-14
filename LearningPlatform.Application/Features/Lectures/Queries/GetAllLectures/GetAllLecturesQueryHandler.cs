using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models;
using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Queries.GetAllLectures;

public sealed class GetAllLecturesQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAllLecturesQuery,
        ResultT<PagedResult<LectureResponse>>>
{
    public async Task<ResultT<PagedResult<LectureResponse>>> Handle(
        GetAllLecturesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Lectures.AsNoTracking();

        if (request.SubjectOfferingId.HasValue)
        {
            query = query.Where(
                x => x.SubjectOfferingId == request.SubjectOfferingId.Value);
        }

        if (request.IsPublished.HasValue)
        {
            query = query.Where(
                x => x.IsPublished == request.IsPublished.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.SubjectOfferingId)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
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
            .ToListAsync(cancellationToken);

        var result = new PagedResult<LectureResponse>(
            items,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return ResultT<PagedResult<LectureResponse>>.Success(result);
    }
}