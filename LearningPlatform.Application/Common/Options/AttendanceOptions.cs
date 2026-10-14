namespace LearningPlatform.Application.Common.Options
{
    public sealed class AttendanceOptions
    {
        public const string SectionName = "Attendance";

        public int LateAfterMinutes { get; set; } = 15;
        public int JoinBeforeStartToleranceMinutes { get; set; } = 30;

        public int FinalizationGracePeriodMinutes { get; set; } = 5;
    }
}
