using LearningPlatform.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Identity;

public class OtpConfiguration : IEntityTypeConfiguration<Otp>
{
    public void Configure(EntityTypeBuilder<Otp> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.PhoneNumber)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(o => o.OtpHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(o => o.ExpiresAt)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.AttemptCount)
            .IsRequired();

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.PhoneNumber);
    }
}
