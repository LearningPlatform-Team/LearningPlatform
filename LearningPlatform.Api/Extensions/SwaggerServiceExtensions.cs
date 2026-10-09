using Microsoft.Extensions.DependencyInjection;
namespace LearningPlatform.Api.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Learning Platform API",
                    Version = "v1",
                    Description = "API documentation for the Learning Platform"
                });
            });
            return services;
        }
    }
}
