using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Domain.Entities.Engagement;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LearningPlatform.Application.Features.Meetings.CreateMeeting
{
    public class CreateMeetingHandler : IRequestHandler<CreateMeetingCommand, ResultT<MeetingDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMeetingProvider _meetingProvidepr;

        public CreateMeetingHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IMeetingProvider meetingProvider)
        {
            _context = context;
            _currentUserService = currentUserService;
            _meetingProvider = meetingProvider;
        }

        public async Task<ResultT<MeetingDto>> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
                return ResultT<MeetingDto>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var currentUserId = _currentUserService.UserId.Value;

            // Verify Subject Offering exists
            var offering = await _context.SubjectOfferings
                .Include(so => so.Subject)
                .FirstOrDefaultAsync(so => so.Id == request.SubjectOfferingId, cancellationToken);

            if (offering == null)
                return ResultT<MeetingDto>.Failure("Subject offering not found.", ErrorType.NotFound);

            var joinCode = GenerateUniqueJoinCode();

            var roomResult = await _meetingProvider.CreateMeetingRoomAsync(
                request.Title,
                joinCode,
                request.ScheduledAt,
                request.MinutesDuration,
                cancellationToken);

            var meeting = new Meeting
            {
                Id = Guid.NewGuid(),
                SubjectOfferingId = request.SubjectOfferingId,
                HostUserId = currentUserId,
                Title = request.Title,
                Description = request.Description ?? string.Empty,
                JoinCode = joinCode,
                Status = request.IsInstant ? MeetingStatus.Live : MeetingStatus.Scheduled,
                ScheduledAt = request.IsInstant ? DateTime.UtcNow : request.ScheduledAt,
                StartedAt = request.IsInstant ? DateTime.UtcNow : null,
                MinutesDuration = request.MinutesDuration,
                ProviderName = roomResult.ProviderName,
                JoinUrl = roomResult.JoinUrl
            };

            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = new MeetingDto(
                meeting.Id,
                meeting.SubjectOfferingId,
                meeting.HostUserId,
                _currentUserService.FullName ?? "Host",
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

        private static string GenerateUniqueJoinCode()
        {
            var randomNum = RandomNumberGenerator.GetInt32(100000, 999999);
            return $"EDU-{randomNum:D6}";
        }
    }
}
