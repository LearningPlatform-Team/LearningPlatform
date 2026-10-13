using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Meetings.CreateMeeting
{
    public record CreateMeetingCommand(
        Guid SubjectOfferingId,
        string Title,
        string? Description,
        DateTime? ScheduledAt,
        int MinutesDuration = 60,
        bool IsInstant = false
    ) : IRequest<ResultT<MeetingDto>>;
}
