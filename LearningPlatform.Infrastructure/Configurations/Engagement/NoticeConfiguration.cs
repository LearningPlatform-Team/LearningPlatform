using LearningPlatform.Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Engagement;

public class NoticeConfiguration : IEntityTypeConfiguration<Notice>
{
    public void Configure(EntityTypeBuilder<Notice> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Body)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(n => n.PublishedAt)
            .IsRequired()
            .HasDefaultValueSql("now() at time zone 'utc'");

        builder.HasIndex(n => n.SubjectOfferingId);

        builder.HasIndex(n => n.PublishedAt);

        // Relationships
        builder.HasOne(n => n.SubjectOffering)
            .WithMany(so => so.Notices)
            .HasForeignKey(n => n.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.AuthorUser)
            .WithMany()
            .HasForeignKey(n => n.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
