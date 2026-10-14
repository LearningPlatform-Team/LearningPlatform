namespace LearningPlatform.Application.Common.Interfaces.BackgroundJobs
{
    public interface IEventReminderJob
    {
        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
