using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations;
public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.YearLevel)
            .IsRequired();

        builder.HasIndex(s => s.Code)
            .IsUnique();

        builder.HasIndex(s => s.YearLevel);

        // Relationships
        builder.HasMany(s => s.Offerings)
            .WithOne(o => o.Subject)
            .HasForeignKey(o => o.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Books)
            .WithOne()
            .OnDelete(DeleteBehavior.Restrict);

    }
}