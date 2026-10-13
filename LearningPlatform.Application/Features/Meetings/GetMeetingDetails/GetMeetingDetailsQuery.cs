using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Meetings.GetMeetingDetails
{
    public record GetMeetingDetailsQuery(Guid MeetingId) : IRequest<ResultT<MeetingDto>>;
}
