using System.Text.Json.Serialization;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;

public class AssignTaCommand : IRequest<Result>
{
    [JsonIgnore]
    public Guid OfferingId { get; set; } = default!;
    public Guid TaProfileId { get; set; } = default!;
}
