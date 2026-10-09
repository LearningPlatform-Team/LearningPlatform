using LearningPlatform.Domain.Entities.Grading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Grading;

public class AssessmentConfiguration : IEntityTypeConfiguration<Assessment>
{
    public void Configure(EntityTypeBuilder<Assessment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.MaxScore)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(a => a.ExamDate);

        builder.HasIndex(a => a.SubjectOfferingId);

        // Relationships
        builder.HasOne(a => a.SubjectOffering)
            .WithMany(so => so.Assessments)
            .HasForeignKey(a => a.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Grades)
            .WithOne(g => g.Assessment)
            .HasForeignKey(g => g.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
