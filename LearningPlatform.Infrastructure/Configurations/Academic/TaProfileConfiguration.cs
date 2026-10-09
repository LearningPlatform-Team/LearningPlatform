using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class TaProfileConfiguration : IEntityTypeConfiguration<TaProfile>
{
    public void Configure(EntityTypeBuilder<TaProfile> builder)
    {
        builder.HasKey(tp => tp.UserId);

        builder.Property(tp => tp.Department)
            .HasMaxLength(100);

        builder.HasIndex(tp => tp.SupervisingDoctorId);

        // Relationships are configured from the User configuration
        builder.HasOne(tp => tp.SupervisingDoctor)
            .WithMany()
            .HasForeignKey(tp => tp.SupervisingDoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(tp => tp.OfferingTas)
            .WithOne(ot => ot.TaProfile)
            .HasForeignKey(ot => ot.TaProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
