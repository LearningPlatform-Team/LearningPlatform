using LearningPlatform.Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Engagement;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.Description)
            .HasMaxLength(1000);

        builder.Property(m => m.JoinCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Status)
            .IsRequired();

        builder.Property(m => m.ScheduledAt);

        builder.Property(m => m.StartedAt);

        builder.Property(m => m.EndedAt);

        builder.Property(m => m.MinutesDuration)
            .IsRequired();

        builder.Property(m => m.ProviderName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.JoinUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(m => m.SubjectOfferingId);

        builder.HasIndex(m => m.Status);

        // Relationships
        builder.HasOne(m => m.SubjectOffering)
            .WithMany(so => so.Meetings)
            .HasForeignKey(m => m.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.HostUser)
            .WithMany()
            .HasForeignKey(m => m.HostUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
