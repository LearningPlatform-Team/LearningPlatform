using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Meetings.StartMeeting
{
    public record StartMeetingCommand(Guid MeetingId) : IRequest<ResultT<MeetingDto>>;
}
