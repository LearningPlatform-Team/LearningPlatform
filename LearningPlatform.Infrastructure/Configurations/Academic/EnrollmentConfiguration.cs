using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.EnrolledAt)
            .IsRequired();

        builder.HasIndex(e => new { e.StudentId, e.OfferingId })
            .IsUnique();

        builder.HasIndex(e => e.OfferingId);

        // Relationships
        builder.HasOne(e => e.Student)
            .WithMany(sp => sp.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SubjectOffering)
            .WithMany(so => so.Enrollments)
            .HasForeignKey(e => e.OfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Grades)
            .WithOne(g => g.Enrollment)
            .HasForeignKey(g => g.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.AttendanceRecords)
            .WithOne(ar => ar.Enrollment)
            .HasForeignKey(ar => ar.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
