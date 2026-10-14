using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.CreateSubjectOffering;

public record CreateSubjectOfferingCommand(
Guid SubjectId,
Guid TermId,
Guid DoctorProfileId
) : IRequest<Result>;
