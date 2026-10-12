using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Events.UpdateEvent;

public sealed class UpdateEventRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public EventType Type { get; set; }
    public DateTime StartsAtUtc { get; set; }
    public DateTime? EndsAtUtc { get; set; }
    public string? Location { get; set; }
    public Guid? SubjectOfferingId { get; set; }
}