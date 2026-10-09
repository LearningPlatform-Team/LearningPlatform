using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Identity;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(u => u.PreferredLanguage)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.DoctorProfile)
            .WithOne(dp => dp.User)
            .HasForeignKey<DoctorProfile>(dp => dp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.StudentProfile)
            .WithOne(sp => sp.User)
            .HasForeignKey<StudentProfile>(sp => sp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.TaProfile)
            .WithOne(tp => tp.User)
            .HasForeignKey<TaProfile>(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notes)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.MyListItems)
            .WithOne(mli => mli.User)
            .HasForeignKey(mli => mli.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.PlaybackLogs)
            .WithOne(pl => pl.User)
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.EventReminders)
            .WithOne(er => er.User)
            .HasForeignKey(er => er.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
