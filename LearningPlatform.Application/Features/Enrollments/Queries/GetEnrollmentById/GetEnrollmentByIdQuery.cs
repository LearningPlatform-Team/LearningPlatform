using LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollmentById;

public record GetEnrollmentByIdQuery(Guid Id)
    : IRequest<ResultT<EnrollmentResponse>>;