namespace LearningPlatform.Application.Common.Interfaces.BackgroundJobs
{
    public interface IAttendanceFinalizationJob
    {

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
