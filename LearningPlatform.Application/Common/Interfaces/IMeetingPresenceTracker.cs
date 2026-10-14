namespace LearningPlatform.Application.Common.Interfaces
{
    public record MeetingParticipant(
        string ConnectionId,
        Guid UserId,
        string UserName,
        string Role,
        bool IsMuted,
        bool IsVideoOn,
        bool IsScreenSharing,
        DateTime JoinedAt
    );

    public interface IMeetingPresenceTracker
    {
        Task<bool> TrackParticipantAsync(string meetingId, MeetingParticipant participant);
        Task<MeetingParticipant?> RemoveParticipantAsync(string meetingId, string connectionId);
        Task<IReadOnlyList<MeetingParticipant>> GetParticipantsAsync(string meetingId);
        Task<MeetingParticipant?> UpdateMediaStateAsync(
            string meetingId,
            string connectionId,
            bool? isMuted = null,
            bool? isVideoOn = null,
            bool? isScreenSharing = null);
        Task<(string? MeetingId, MeetingParticipant? Participant)> DisconnectConnectionAsync(string connectionId);
    }
}
