using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Meetings.JoinMeeting
{
    public record JoinMeetingResultDto(
        Guid MeetingId,
        string Title,
        string JoinUrl,
        string ProviderName,
        MeetingStatus Status,
        bool IsHost,
        bool AttendanceMarked,
        string Message
    );
}
