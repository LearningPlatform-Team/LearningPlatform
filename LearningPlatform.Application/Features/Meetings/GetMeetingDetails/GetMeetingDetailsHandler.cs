using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Meetings.GetMeetingDetails
{
    public class GetMeetingDetailsHandler : IRequestHandler<GetMeetingDetailsQuery, ResultT<MeetingDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetMeetingDetailsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultT<MeetingDto>> Handle(GetMeetingDetailsQuery request, CancellationToken cancellationToken)
        {
            var meeting = await _context.Meetings
                .Include(m => m.HostUser)
                .FirstOrDefaultAsync(m => m.Id == request.MeetingId, cancellationToken);

            if (meeting == null)
                return ResultT<MeetingDto>.Failure("Meeting not found.", ErrorType.NotFound);

            var dto = new MeetingDto(
                meeting.Id,
                meeting.SubjectOfferingId,
                meeting.HostUserId,
                meeting.HostUser?.FullName ?? "Host",
                meeting.Title,
                meeting.Description,
                meeting.JoinCode,
                meeting.Status,
                meeting.ScheduledAt,
                meeting.StartedAt,
                meeting.EndedAt,
                meeting.MinutesDuration,
                meeting.ProviderName,
                meeting.JoinUrl
            );

            return ResultT<MeetingDto>.Success(dto);
        }
    }
}
