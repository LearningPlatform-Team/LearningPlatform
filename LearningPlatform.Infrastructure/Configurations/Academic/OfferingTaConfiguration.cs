using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class OfferingTaConfiguration : IEntityTypeConfiguration<OfferingTa>
{
    public void Configure(EntityTypeBuilder<OfferingTa> builder)
    {
        builder.HasKey(ot => new { ot.OfferingId, ot.TaProfileId });

        builder.Property(ot => ot.AssignedAt)
            .IsRequired();

        builder.HasIndex(ot => ot.TaProfileId);

        // Relationships
        builder.HasOne(ot => ot.Offering)
            .WithMany(so => so.OfferingTas)
            .HasForeignKey(ot => ot.OfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ot => ot.TaProfile)
            .WithMany(tp => tp.OfferingTas)
            .HasForeignKey(ot => ot.TaProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
