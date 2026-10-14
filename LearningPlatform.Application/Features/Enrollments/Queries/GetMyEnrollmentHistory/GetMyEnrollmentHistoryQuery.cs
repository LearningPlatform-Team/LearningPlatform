using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;

public record GetMyEnrollmentHistoryQuery() : IRequest<ResultT<List<EnrollmentHistoryResponse>>>;
