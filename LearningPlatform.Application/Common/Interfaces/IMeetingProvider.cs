using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Common.Interfaces
{
    public record MeetingRoomResult(string RoomId, string JoinUrl, string ProviderName);

    public interface IMeetingProvider
    {
        Task<MeetingRoomResult> CreateMeetingRoomAsync(
            string title,
            string joinCode,
            DateTime? scheduledAt = null,
            int durationMinutes = 60,
            CancellationToken cancellationToken = default);
    }
}
