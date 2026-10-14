using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Meetings.StartMeeting
{
    public class StartMeetingHandler : IRequestHandler<StartMeetingCommand, ResultT<MeetingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public StartMeetingHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResultT<MeetingDto>> Handle(StartMeetingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
                return ResultT<MeetingDto>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var currentUserId = _currentUserService.UserId.Value;

            var meeting = await _context.Meetings
                .Include(m => m.HostUser)
                .FirstOrDefaultAsync(m => m.Id == request.MeetingId, cancellationToken);

            if (meeting == null)
                return ResultT<MeetingDto>.Failure("Meeting not found.", ErrorType.NotFound);

            // Only the host (or an admin) can start the meeting
            var isHost = meeting.HostUserId == currentUserId;
            var isAdmin = _currentUserService.IsInRole("Admin");

            if (!isHost && !isAdmin)
                return ResultT<MeetingDto>.Failure("Only the host or an administrator can start this meeting.", ErrorType.Unauthorized);

            if (meeting.Status == MeetingStatus.Ended || meeting.Status == MeetingStatus.Cancelled)
                return ResultT<MeetingDto>.Failure("Cannot start a meeting that has already ended or been cancelled.", ErrorType.Conflict);

            if (meeting.Status != MeetingStatus.Live)
            {
                meeting.Status = MeetingStatus.Live;
                meeting.StartedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }

            var dto = new MeetingDto(
                meeting.Id,
                meeting.SubjectOfferingId,
                meeting.HostUserId,
                meeting.HostUser?.FullName ?? _currentUserService.FullName ?? "Host",
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
