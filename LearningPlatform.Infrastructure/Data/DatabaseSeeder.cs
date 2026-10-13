using LearningPlatform.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        string[] roleNames = { "Student", "Doctor", "TA", "Admin" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' seeded successfully.", roleName);
                }
                else
                {
                    logger.LogWarning("Failed to seed role '{RoleName}': {Errors}", 
                        roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        const string adminEmail = "admin@learnora.com";
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                FullName = "System Administrator",
                Email = adminEmail,
                UserName = adminEmail,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Default Admin user '{Email}' seeded successfully.", adminEmail);
            }
            else
            {
                logger.LogWarning("Failed to seed default Admin: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }

    public static async Task SeedSampleAcademicDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        var sampleOfferingId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        if (!await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync(db.SubjectOfferings, so => so.Id == sampleOfferingId))
        {
            var adminUser = await userManager.FindByEmailAsync("admin@learnora.com");
            if (adminUser == null) return;

            // 1. Ensure DoctorProfile for Admin
            var doctorProfile = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.DoctorProfiles, dp => dp.UserId == adminUser.Id);
            if (doctorProfile == null)
            {
                doctorProfile = new LearningPlatform.Domain.Entities.Academic.DoctorProfile
                {
                    UserId = adminUser.Id,
                    AcademicTitle = "Professor"
                };
                db.DoctorProfiles.Add(doctorProfile);
                await db.SaveChangesAsync();
            }

            // 2. Ensure Academic Term
            var termId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var term = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.AcademicTerms, t => t.Id == termId);
            if (term == null)
            {
                term = new LearningPlatform.Domain.Entities.Academic.AcademicTerm
                {
                    Id = termId,
                    AcademicYear = "2026-2027",
                    TermNumber = 1,
                    StartDate = new DateOnly(2026, 9, 1),
                    EndDate = new DateOnly(2027, 6, 30),
                    IsCurrent = true
                };
                db.AcademicTerms.Add(term);
                await db.SaveChangesAsync();
            }

            // 3. Ensure Subject
            var subjectId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var subject = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.Subjects, s => s.Id == subjectId);
            if (subject == null)
            {
                subject = new LearningPlatform.Domain.Entities.Academic.Subject
                {
                    Id = subjectId,
                    Code = "CS101",
                    Name = "Computer Science 101",
                    Description = "Introduction to Computer Science and Programming",
                    YearLevel = LearningPlatform.Domain.Enums.YearLevel.First
                };
                db.Subjects.Add(subject);
                await db.SaveChangesAsync();
            }

            // 4. Create Subject Offering
            var offering = new LearningPlatform.Domain.Entities.Academic.SubjectOffering
            {
                Id = sampleOfferingId,
                SubjectId = subject.Id,
                TermId = term.Id,
                DoctorProfileId = doctorProfile.UserId,
                IsActive = true
            };
            db.SubjectOfferings.Add(offering);
            await db.SaveChangesAsync();
            logger.LogInformation("Sample SubjectOffering '{OfferingId}' (CS101) seeded successfully.", sampleOfferingId);
        }
    }
}
