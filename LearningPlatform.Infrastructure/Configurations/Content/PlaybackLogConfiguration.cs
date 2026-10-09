using LearningPlatform.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Content;

public class PlaybackLogConfiguration : IEntityTypeConfiguration<PlaybackLog>
{
    public void Configure(EntityTypeBuilder<PlaybackLog> builder)
    {
        builder.HasKey(pl => pl.Id);

        builder.Property(pl => pl.RequestedAt)
            .IsRequired();

        builder.HasIndex(pl => new { pl.UserId, pl.LectureId });

        builder.HasIndex(pl => pl.LectureId);

        // Relationships
        builder.HasOne(pl => pl.User)
            .WithMany(u => u.PlaybackLogs)
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pl => pl.Lecture)
            .WithMany(l => l.PlaybackLogs)
            .HasForeignKey(pl => pl.LectureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
