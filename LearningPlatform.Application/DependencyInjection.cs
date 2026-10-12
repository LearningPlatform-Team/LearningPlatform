
using FluentValidation;
using LearningPlatform.Application.Common.Behaviors;
using LearningPlatform.Application.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services,IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.Configure<AttendanceOptions>(configuration.GetSection(AttendanceOptions.SectionName));
            services.AddValidatorsFromAssembly(assembly);
            return services;
        }
    }
}
