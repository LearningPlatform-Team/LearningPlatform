using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.RemoveTaFromOffering;

public record RemoveTaFromOfferingCommand(
    Guid SubjectOfferingId,
    Guid TaProfileId
) : IRequest<Result>;