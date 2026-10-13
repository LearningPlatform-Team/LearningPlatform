using LearningPlatform.Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Engagement;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Type)
            .IsRequired();

        builder.Property(e => e.StartsAtUtc)
            .IsRequired();

        builder.HasIndex(e => e.StartsAtUtc);
       
        builder.HasIndex(eventItem => new
        {
            eventItem.SubjectOfferingId,
            eventItem.StartsAtUtc
        });

        builder.HasIndex(e => e.Type);

        // Relationships
        builder.HasOne(e => e.SubjectOffering)
            .WithMany(so => so.Events)
            .HasForeignKey(e => e.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
