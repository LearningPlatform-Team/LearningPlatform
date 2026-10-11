using Hangfire;
using Hangfire.PostgreSql;
using LearningPlatform.Application.Common.Interfaces.BackgroundJobs;
using LearningPlatform.Infrastructure.BackgroundJobs.Attendance;

namespace LearningPlatform.Api.Extensions;

public static class HangfireServiceExtensions
{
    public static IServiceCollection AddHangfireServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("HangfireConnection")
            ?? throw new InvalidOperationException(
                "The 'HangfireConnection' connection string was not found.");

        services.AddScoped<
            IAttendanceFinalizationJob,
            AttendanceFinalizationJob>();

        services.AddHangfire(hangfireConfiguration =>
        {
            hangfireConfiguration
                .SetDataCompatibilityLevel(
                    CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    options =>
                    {
                        options.UseNpgsqlConnection(connectionString);
                    },
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "hangfire",
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval =
                            TimeSpan.FromSeconds(15)
                    });
        });

        services.AddHangfireServer(options =>
        {
            options.ServerName =
                $"{Environment.MachineName}:learning-platform";

            options.WorkerCount = Math.Max(
                Environment.ProcessorCount,
                2);
        });

        return services;
    }
}