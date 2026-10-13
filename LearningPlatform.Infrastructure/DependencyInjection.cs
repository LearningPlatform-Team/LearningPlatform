
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Interfaces.BackgroundJobs;
using LearningPlatform.Domain.Entities.Identity;
using LearningPlatform.Infrastructure.BackgroundJobs.Attendance;
using LearningPlatform.Infrastructure.Data;
using LearningPlatform.Infrastructure.Messaging.Email;
using LearningPlatform.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LearningPlatform.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(
                            connectionString,
                            b => 
                            {
                                b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                                b.CommandTimeout(300);
                            }));
            
            services.AddScoped<IApplicationDbContext>(provider =>
                    provider.GetRequiredService<AppDbContext>());

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.AddHttpClient();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IExternalAuthService, ExternalAuthService>();

<<<<<<< HEAD
            // Realtime Meeting Services & Infrastructure
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, LearningPlatform.Infrastructure.Services.CurrentUserService>();
            services.AddScoped<IMeetingProvider, LearningPlatform.Infrastructure.Services.GoogleMeetMeetingProvider>();
            services.AddSingleton<IMeetingPresenceTracker, LearningPlatform.Infrastructure.Realtime.Presence.MeetingPresenceTracker>();
            services.AddSignalR();
=======
>>>>>>> origin/main

            return services;
        }
    }
}
