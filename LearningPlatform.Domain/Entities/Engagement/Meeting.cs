using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Domain.Enums;
namespace LearningPlatform.Domain.Entities.Engagement
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public Guid HostUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string JoinCode { get; set; } = string.Empty;
        public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;
        public DateTime? ScheduledAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int MinutesDuration { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public string JoinUrl { get; set; } = string.Empty;

        public User HostUser { get; set; } = null!;
        public SubjectOffering SubjectOffering { get; set; } = null!;
    }
}
