using LearningPlatform.Application.Features.AcademicTerms.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.AcademicTerms.Queries.GetAllAcademicTerms;

public record GetAllAcademicTermsQuery
    : IRequest<ResultT<List<AcademicTermResponse>>>;