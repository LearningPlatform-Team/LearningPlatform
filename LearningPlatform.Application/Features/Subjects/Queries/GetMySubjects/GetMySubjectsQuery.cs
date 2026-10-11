using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetMySubjects;

public record GetMySubjectsQuery
 : IRequest<ResultT<List<MySubjectResponse>>>;
