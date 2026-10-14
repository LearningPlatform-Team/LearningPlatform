using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;

public record GetSubjectByIdQuery(Guid Id) : IRequest<ResultT<SubjectResponse>>;
