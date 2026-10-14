
using LearningPlatform.Application.Common.Models;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;

public record GetSubjectOfferingsQuery(
    Guid? TermId = null,
    Guid? SubjectId = null,
    Guid? DoctorProfileId = null,
    bool? IsActive = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedResult<SubjectOfferingResponse>>;