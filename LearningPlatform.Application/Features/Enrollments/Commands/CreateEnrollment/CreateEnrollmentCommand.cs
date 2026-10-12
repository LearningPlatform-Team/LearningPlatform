using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;

public record CreateEnrollmentCommand(Guid StudentId, Guid OfferingId) : IRequest<Result>;
