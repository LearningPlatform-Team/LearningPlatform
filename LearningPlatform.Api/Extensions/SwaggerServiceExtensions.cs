using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace LearningPlatform.Api.Extensions
{
    public static class SwaggerServiceExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Learning Platform (Learnora) API",
                    Version = "v1",
                    Description = "Comprehensive API documentation for Learnora Mobile App & Web Frontend. Use the Authorize button to authenticate with your Bearer token."
                });

                // Add JWT Bearer Authorization button in Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT Bearer token only (no need to type Bearer, Swagger adds it automatically)"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Include XML Comments from API assembly
                var apiXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var apiXmlPath = Path.Combine(AppContext.BaseDirectory, apiXmlFile);
                if (File.Exists(apiXmlPath))
                {
                    options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
                }

                // Include XML Comments from Application assembly
                var appXmlPath = Path.Combine(AppContext.BaseDirectory, "LearningPlatform.Application.xml");
                if (File.Exists(appXmlPath))
                {
                    options.IncludeXmlComments(appXmlPath);
                }
            });
            return services;
        }
    }
}
