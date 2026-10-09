using LearningPlatform.Domain.Entities.Attendance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Attendance;

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.SessionDate)
            .IsRequired();

        builder.Property(cs => cs.StartTime)
            .IsRequired();

        builder.Property(cs => cs.EndTime)
            .IsRequired();

        builder.HasIndex(cs => cs.SubjectOfferingId);

        builder.HasIndex(cs => new { cs.SubjectOfferingId, cs.SessionDate });

        // Relationships
        builder.HasOne(cs => cs.SubjectOffering)
            .WithMany(so => so.ClassSessions)
            .HasForeignKey(cs => cs.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(cs => cs.AttendanceRecords)
            .WithOne(ar => ar.ClassSession)
            .HasForeignKey(ar => ar.ClassSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
