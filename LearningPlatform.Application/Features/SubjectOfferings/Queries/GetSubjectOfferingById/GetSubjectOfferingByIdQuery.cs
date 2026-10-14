using LearningPlatform.Application.Features.SubjectOfferings.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferingById;

public record GetSubjectOfferingByIdQuery(Guid Id)
    : IRequest<ResultT<SubjectOfferingResponse>>;