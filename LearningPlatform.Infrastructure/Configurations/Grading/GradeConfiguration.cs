using LearningPlatform.Domain.Entities.Grading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Grading;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Score)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(g => g.GradedAt)
            .IsRequired();

        builder.HasIndex(g => new { g.EnrollmentId, g.AssessmentId })
            .IsUnique();

        builder.HasIndex(g => g.AssessmentId);

        builder.HasIndex(g => g.GradedByUserId);

        // Relationships
        builder.HasOne(g => g.Enrollment)
            .WithMany(e => e.Grades)
            .HasForeignKey(g => g.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.Assessment)
            .WithMany(a => a.Grades)
            .HasForeignKey(g => g.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(g => g.GradedByUser)
            .WithMany()
            .HasForeignKey(g => g.GradedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
