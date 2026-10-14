using LearningPlatform.Application.Common.Models;
using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Lectures.Queries.GetAllLectures;

public sealed record GetAllLecturesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? SubjectOfferingId = null,
    bool? IsPublished = null
) : IRequest<ResultT<PagedResult<LectureResponse>>>;