using LearningPlatform.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Content;

public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(l => l.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(l => l.DurationMinutes)
            .IsRequired();

        builder.Property(l => l.VideoProvider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(l => l.VideoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.SortOrder)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.IsPublished)
            .IsRequired();

        builder.HasIndex(l => l.SubjectOfferingId);

        builder.HasIndex(l => l.CreatedAt);

        // Relationships
        builder.HasOne(l => l.Offering)
            .WithMany(so => so.Lectures)
            .HasForeignKey(l => l.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.UploadedBy)
            .WithMany()
            .HasForeignKey(l => l.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.PlaybackLogs)
            .WithOne(pl => pl.Lecture)
            .HasForeignKey(pl => pl.LectureId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(l => l.MyListItems)
            .WithOne(mli => mli.Lecture)
            .HasForeignKey(mli => mli.LectureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
