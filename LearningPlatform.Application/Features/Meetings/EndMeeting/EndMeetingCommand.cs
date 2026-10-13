using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Meetings.EndMeeting
{
    public record EndMeetingCommand(Guid MeetingId) : IRequest<ResultT<MeetingDto>>;
}
