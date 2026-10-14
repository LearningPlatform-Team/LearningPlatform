using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Attendance;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Meetings.JoinMeeting
{
    public class JoinMeetingHandler : IRequestHandler<JoinMeetingCommand, ResultT<JoinMeetingResultDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public JoinMeetingHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResultT<JoinMeetingResultDto>> Handle(JoinMeetingCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.UserId.HasValue)
                return ResultT<JoinMeetingResultDto>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var currentUserId = _currentUserService.UserId.Value;

            // Find meeting by ID or JoinCode
            var query = _context.Meetings.AsQueryable();

            if (request.MeetingId.HasValue)
            {
                query = query.Where(m => m.Id == request.MeetingId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(request.JoinCode))
            {
                var normalizedCode = request.JoinCode.Trim().ToUpperInvariant();
                query = query.Where(m => m.JoinCode.ToUpper() == normalizedCode);
            }

            var meeting = await query.FirstOrDefaultAsync(cancellationToken);

            if (meeting == null)
                return ResultT<JoinMeetingResultDto>.Failure("Meeting not found.", ErrorType.NotFound);

            var isHost = meeting.HostUserId == currentUserId;

            // 1. Host flow: Host / Doctor can join anytime with or without a participant code
            if (isHost)
            {
                var hostResult = new JoinMeetingResultDto(
                    meeting.Id,
                    meeting.Title,
                    meeting.JoinUrl,
                    meeting.ProviderName,
                    meeting.Status,
                    IsHost: true,
                    AttendanceMarked: false,
                    Message: "Host entered meeting room successfully."
                );

                return ResultT<JoinMeetingResultDto>.Success(hostResult);
            }

            // 2. Participant / Student flow:
            // Must supply the participant join code
            if (string.IsNullOrWhiteSpace(request.JoinCode) ||
                !string.Equals(meeting.JoinCode.Trim(), request.JoinCode.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return ResultT<JoinMeetingResultDto>.Failure("Valid participant join code is required to enter this meeting.", ErrorType.Validation);
            }

            // Meeting must be Live
            if (meeting.Status != MeetingStatus.Live)
            {
                return ResultT<JoinMeetingResultDto>.Failure("Meeting has not started yet. Please wait for the host to start the call.", ErrorType.Conflict);
            }

            // Check and mark attendance if user is enrolled in the subject offering
            var attendanceMarked = false;
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == currentUserId && e.OfferingId == meeting.SubjectOfferingId, cancellationToken);

            if (enrollment != null)
            {
                var today = DateTime.UtcNow.Date;
                var session = await _context.ClassSessions
                    .FirstOrDefaultAsync(cs => cs.SubjectOfferingId == meeting.SubjectOfferingId && cs.SessionDate.Date == today, cancellationToken);

                if (session == null)
                {
                    session = new ClassSession
                    {
                        Id = Guid.NewGuid(),
                        SubjectOfferingId = meeting.SubjectOfferingId,
                        SessionDate = DateTime.UtcNow,
                        StartTime = DateTime.UtcNow,
                        EndTime = DateTime.UtcNow.AddMinutes(meeting.MinutesDuration > 0 ? meeting.MinutesDuration : 60)
                    };
                    _context.ClassSessions.Add(session);
                }

                var existingAttendance = await _context.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.ClassSessionId == session.Id && a.EnrollmentId == enrollment.Id, cancellationToken);

                if (existingAttendance == null)
                {
                    var record = new AttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        ClassSessionId = session.Id,
                        EnrollmentId = enrollment.Id,
                        Status = AttendanceStatus.Present,
                        MarkedAt = DateTime.UtcNow,
                        Source = AttendanceSource.Automatic
                    };
                    _context.AttendanceRecords.Add(record);
                    await _context.SaveChangesAsync(cancellationToken);
                    attendanceMarked = true;
                }
            }

            var participantResult = new JoinMeetingResultDto(
                meeting.Id,
                meeting.Title,
                meeting.JoinUrl,
                meeting.ProviderName,
                meeting.Status,
                IsHost: false,
                AttendanceMarked: attendanceMarked,
                Message: attendanceMarked
                    ? "Participant joined successfully. Attendance automatically recorded."
                    : "Participant joined meeting successfully."
            );

            return ResultT<JoinMeetingResultDto>.Success(participantResult);
        }
    }
}
