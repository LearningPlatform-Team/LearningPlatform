
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IApplicationDbContext>(provider =>
                    provider.GetRequiredService<AppDbContext>());
            return services;
        }
    }
}
