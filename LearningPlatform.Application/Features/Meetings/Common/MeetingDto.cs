using LearningPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Meetings.Common
{
    public record MeetingDto(
        Guid Id,
        Guid SubjectOfferingId,
        Guid HostUserId,
        string HostName,
        string Title,
        string Description,
        string JoinCode,
        MeetingStatus Status,
        DateTime? ScheduledAt,
        DateTime? StartedAt,
        DateTime? EndedAt,
        int MinutesDuration,
        string ProviderName,
        string JoinUrl
    );
}
