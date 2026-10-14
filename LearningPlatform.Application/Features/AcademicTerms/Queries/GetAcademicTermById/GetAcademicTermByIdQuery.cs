using LearningPlatform.Application.Features.AcademicTerms.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.AcademicTerms.Queries.GetAcademicTermById;

public record GetAcademicTermByIdQuery(Guid Id)
    : IRequest<ResultT<AcademicTermResponse>>;