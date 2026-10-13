using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Meetings.JoinMeeting
{
    public record JoinMeetingCommand(
        string? JoinCode = null,
        Guid? MeetingId = null
    ) : IRequest<ResultT<JoinMeetingResultDto>>;
}
