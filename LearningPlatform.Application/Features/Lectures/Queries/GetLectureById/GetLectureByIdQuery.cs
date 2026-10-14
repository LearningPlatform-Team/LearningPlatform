using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Lectures.Queries.GetLectureById;

public sealed record GetLectureByIdQuery(Guid Id)
    : IRequest<ResultT<LectureResponse>>;