using Hangfire;
using LearningPlatform.Application.Common.Interfaces.BackgroundJobs;

namespace LearningPlatform.Api.Extensions;

public static class HangfireApplicationExtensions
{
    private const string AttendanceFinalizationJobId =
        "finalize-ended-meeting-attendance";
    private const string EventReminderJobId =
       "process-event-reminders";
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        recurringJobManager.AddOrUpdate<IAttendanceFinalizationJob>(
            recurringJobId: AttendanceFinalizationJobId,
            methodCall: job => job.ExecuteAsync(CancellationToken.None), cronExpression:"*/5 * * * *",
            options: new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });
        recurringJobManager.AddOrUpdate<IEventReminderJob>(recurringJobId:EventReminderJobId,
            methodCall:job => job.ExecuteAsync(CancellationToken.None),cronExpression:"* * * * *",
            options:new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        return app;
    }
}