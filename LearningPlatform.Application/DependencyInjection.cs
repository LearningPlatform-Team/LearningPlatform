
using FluentValidation;
using LearningPlatform.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(assembly);
            return services;
        }
    }
}
