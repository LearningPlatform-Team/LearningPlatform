using LearningPlatform.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Content;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.FileStorageKey)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(b => b.SubjectId);

        builder.HasIndex(b => b.UploadedByUserId);

        // Relationships
        builder.HasOne(b => b.Subject)
            .WithMany(s => s.Books)
            .HasForeignKey(b => b.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.UploadedByUser)
            .WithMany()
            .HasForeignKey(b => b.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
