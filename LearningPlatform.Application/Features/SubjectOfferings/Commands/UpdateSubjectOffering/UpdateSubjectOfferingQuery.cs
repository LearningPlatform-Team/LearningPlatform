using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;

public record UpdateSubjectOfferingCommand(
Guid Id,
Guid SubjectId,
Guid TermId,
Guid DoctorProfileId,
bool IsActive
) : IRequest<Result>;
