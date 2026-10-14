using LearningPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LearningPlatform.Infrastructure.Hub
{
    [Authorize]
    public class MeetingHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IMeetingPresenceTracker _presenceTracker;

        public MeetingHub(IMeetingPresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public async Task JoinMeetingRoom(string meetingId, string? requestedRole = null)
        {
            var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                            Context.User?.FindFirstValue("sub");
            var userId = Guid.TryParse(userIdStr, out var parsedId) ? parsedId : Guid.Empty;

            var userName = Context.User?.FindFirstValue(ClaimTypes.Name) ??
                           Context.User?.FindFirstValue("name") ??
                           Context.User?.FindFirstValue(ClaimTypes.Email) ??
                           "Participant";

            var role = requestedRole ?? (Context.User?.IsInRole("Doctor") == true ? "Doctor" : "Student");

            var participant = new MeetingParticipant(
                ConnectionId: Context.ConnectionId,
                UserId: userId,
                UserName: userName,
                Role: role,
                IsMuted: false,
                IsVideoOn: true,
                IsScreenSharing: false,
                JoinedAt: DateTime.UtcNow
            );

            await Groups.AddToGroupAsync(Context.ConnectionId, meetingId);
            await _presenceTracker.TrackParticipantAsync(meetingId, participant);

            // Notify others in room that this participant joined
            await Clients.OthersInGroup(meetingId).SendAsync("UserJoined", participant);

            // Send caller the current full roster of participants in the room
            var currentRoster = await _presenceTracker.GetParticipantsAsync(meetingId);
            await Clients.Caller.SendAsync("MeetingRoster", currentRoster);
        }

        public async Task LeaveMeetingRoom(string meetingId)
        {
            var removed = await _presenceTracker.RemoveParticipantAsync(meetingId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, meetingId);

            if (removed != null)
            {
                await Clients.OthersInGroup(meetingId).SendAsync("UserLeft", new
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = removed.UserId,
                    UserName = removed.UserName
                });
            }
        }

        public async Task ToggleAudio(string meetingId, bool isMuted)
        {
            var updated = await _presenceTracker.UpdateMediaStateAsync(meetingId, Context.ConnectionId, isMuted: isMuted);
            if (updated != null)
            {
                await Clients.Group(meetingId).SendAsync("UserAudioChanged", new
                {
                    ConnectionId = Context.ConnectionId,
                    IsMuted = isMuted
                });
            }
        }

        public async Task ToggleVideo(string meetingId, bool isVideoOn)
        {
            var updated = await _presenceTracker.UpdateMediaStateAsync(meetingId, Context.ConnectionId, isVideoOn: isVideoOn);
            if (updated != null)
            {
                await Clients.Group(meetingId).SendAsync("UserVideoChanged", new
                {
                    ConnectionId = Context.ConnectionId,
                    IsVideoOn = isVideoOn
                });
            }
        }

        public async Task ToggleScreenShare(string meetingId, bool isScreenSharing)
        {
            var updated = await _presenceTracker.UpdateMediaStateAsync(meetingId, Context.ConnectionId, isScreenSharing: isScreenSharing);
            if (updated != null)
            {
                await Clients.Group(meetingId).SendAsync("UserScreenShareChanged", new
                {
                    ConnectionId = Context.ConnectionId,
                    IsScreenSharing = isScreenSharing
                });
            }
        }

        public async Task SendMeetingMessage(string meetingId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var userName = Context.User?.FindFirstValue(ClaimTypes.Name) ??
                           Context.User?.FindFirstValue("name") ??
                           "Participant";

            await Clients.Group(meetingId).SendAsync("ReceiveMeetingMessage", new
            {
                ConnectionId = Context.ConnectionId,
                SenderName = userName,
                Message = message,
                Timestamp = DateTime.UtcNow
            });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var (meetingId, participant) = await _presenceTracker.DisconnectConnectionAsync(Context.ConnectionId);
            if (meetingId != null && participant != null)
            {
                await Clients.OthersInGroup(meetingId).SendAsync("UserLeft", new
                {
                    ConnectionId = Context.ConnectionId,
                    UserId = participant.UserId,
                    UserName = participant.UserName
                });
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
