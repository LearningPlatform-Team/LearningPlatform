using LearningPlatform.Application.Common.Models;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;

public record GetEnrollmentsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? StudentId = null,
    Guid? OfferingId = null,
    EnrollmentStatus? Status = null
) : IRequest<ResultT<PagedResult<EnrollmentResponse>>>;