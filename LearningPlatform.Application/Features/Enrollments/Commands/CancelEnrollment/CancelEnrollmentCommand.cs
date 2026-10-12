using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Commands.CancelEnrollment;

public record CancelEnrollmentCommand(Guid EnrollmentId) : IRequest<Result>;

