using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class AcademicTermConfiguration : IEntityTypeConfiguration<AcademicTerm>
{
    public void Configure(EntityTypeBuilder<AcademicTerm> builder)
    {
        builder.HasKey(at => at.Id);

        builder.Property(at => at.AcademicYear)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(at => at.TermNumber)
            .IsRequired();

        builder.Property(at => at.StartDate)
            .IsRequired();

        builder.Property(at => at.EndDate)
            .IsRequired();

        builder.Property(at => at.IsCurrent)
            .IsRequired();

        builder.HasIndex(at => at.AcademicYear);

        builder.HasIndex(at => at.IsCurrent);

        // Relationships
        builder.HasMany(at => at.Offerings)
            .WithOne(so => so.Term)
            .HasForeignKey(so => so.TermId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
