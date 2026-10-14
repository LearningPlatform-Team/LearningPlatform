using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectLectures;

public record GetSubjectLecturesQuery(Guid SubjectId)
    : IRequest<ResultT<List<LectureResponse>>>;