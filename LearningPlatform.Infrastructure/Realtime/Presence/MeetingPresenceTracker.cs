using LearningPlatform.Application.Common.Interfaces;
using System.Collections.Concurrent;

namespace LearningPlatform.Infrastructure.Realtime.Presence
{
    public class MeetingPresenceTracker : IMeetingPresenceTracker
    {
        // MeetingId -> (ConnectionId -> MeetingParticipant)
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, MeetingParticipant>> _rooms = new();

        public Task<bool> TrackParticipantAsync(string meetingId, MeetingParticipant participant)
        {
            var participants = _rooms.GetOrAdd(meetingId, _ => new ConcurrentDictionary<string, MeetingParticipant>());
            participants[participant.ConnectionId] = participant;
            return Task.FromResult(true);
        }

        public Task<MeetingParticipant?> RemoveParticipantAsync(string meetingId, string connectionId)
        {
            if (_rooms.TryGetValue(meetingId, out var participants))
            {
                participants.TryRemove(connectionId, out var removed);
                if (participants.IsEmpty)
                {
                    _rooms.TryRemove(meetingId, out _);
                }
                return Task.FromResult(removed);
            }

            return Task.FromResult<MeetingParticipant?>(null);
        }

        public Task<IReadOnlyList<MeetingParticipant>> GetParticipantsAsync(string meetingId)
        {
            if (_rooms.TryGetValue(meetingId, out var participants))
            {
                IReadOnlyList<MeetingParticipant> list = participants.Values.ToList();
                return Task.FromResult(list);
            }

            IReadOnlyList<MeetingParticipant> empty = Array.Empty<MeetingParticipant>();
            return Task.FromResult(empty);
        }

        public Task<MeetingParticipant?> UpdateMediaStateAsync(
            string meetingId,
            string connectionId,
            bool? isMuted = null,
            bool? isVideoOn = null,
            bool? isScreenSharing = null)
        {
            if (_rooms.TryGetValue(meetingId, out var participants) &&
                participants.TryGetValue(connectionId, out var current))
            {
                var updated = current with
                {
                    IsMuted = isMuted ?? current.IsMuted,
                    IsVideoOn = isVideoOn ?? current.IsVideoOn,
                    IsScreenSharing = isScreenSharing ?? current.IsScreenSharing
                };

                participants[connectionId] = updated;
                return Task.FromResult<MeetingParticipant?>(updated);
            }

            return Task.FromResult<MeetingParticipant?>(null);
        }

        public Task<(string? MeetingId, MeetingParticipant? Participant)> DisconnectConnectionAsync(string connectionId)
        {
            foreach (var kvp in _rooms)
            {
                if (kvp.Value.TryRemove(connectionId, out var removed))
                {
                    if (kvp.Value.IsEmpty)
                    {
                        _rooms.TryRemove(kvp.Key, out _);
                    }
                    return Task.FromResult<(string?, MeetingParticipant?)>((kvp.Key, removed));
                }
            }

            return Task.FromResult<(string?, MeetingParticipant?)>((null, null));
        }
    }
}
