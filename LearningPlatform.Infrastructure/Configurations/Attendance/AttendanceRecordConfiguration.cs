using LearningPlatform.Domain.Entities.Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Attendance;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.Status)
            .IsRequired();

        builder.Property(ar => ar.MarkedAt);

        builder.HasIndex(ar => new { ar.ClassSessionId, ar.EnrollmentId })
            .IsUnique();

        builder.HasIndex(ar => ar.EnrollmentId);

        builder.HasIndex(ar => ar.MarkedByUserId);

        // Relationships
        builder.HasOne(ar => ar.ClassSession)
            .WithMany(cs => cs.AttendanceRecords)
            .HasForeignKey(ar => ar.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ar => ar.Enrollment)
            .WithMany()
            .HasForeignKey(ar => ar.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
