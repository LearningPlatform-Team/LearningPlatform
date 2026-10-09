using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class DoctorProfileConfiguration : IEntityTypeConfiguration<DoctorProfile>
{
    public void Configure(EntityTypeBuilder<DoctorProfile> builder)
    {
        builder.HasKey(dp => dp.UserId);

        builder.Property(dp => dp.AcademicTitle)
            .HasMaxLength(100);

        // Relationships are configured from the User configuration
        builder.HasMany(dp => dp.Offerings)
            .WithOne(so => so.Doctor)
            .HasForeignKey(so => so.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
