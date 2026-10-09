using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class SubjectOfferingConfiguration
    : IEntityTypeConfiguration<SubjectOffering>
{
    public void Configure(EntityTypeBuilder<SubjectOffering> builder)
    {
        builder.HasKey(so => so.Id);

        builder.Property(so => so.IsActive)
            .IsRequired();

        // Indexes
        builder.HasIndex(so => new { so.SubjectId, so.TermId })
            .IsUnique();

        // Relationships
        builder.HasOne(so => so.Subject)
            .WithMany(s => s.Offerings)
            .HasForeignKey(so => so.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Doctor)
            .WithMany(dp => dp.Offerings)
            .HasForeignKey(so => so.DoctorProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(so => so.Term)
          .WithMany(at => at.Offerings)
          .HasForeignKey(so => so.TermId)
          .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(so => so.Enrollments)
            .WithOne(e => e.SubjectOffering)
            .HasForeignKey(e => e.OfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.OfferingTas)
            .WithOne(ot => ot.Offering)
            .HasForeignKey(ot => ot.OfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.ClassSessions)
            .WithOne(cs => cs.SubjectOffering)
            .HasForeignKey(cs => cs.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.Lectures)
            .WithOne(l => l.Offering)
            .HasForeignKey(l => l.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.Assessments)
            .WithOne(a => a.SubjectOffering)
            .HasForeignKey(a => a.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.Meetings)
            .WithOne(m => m.SubjectOffering)
            .HasForeignKey(m => m.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.Events)
            .WithOne(e => e.SubjectOffering)
            .HasForeignKey(e => e.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(so => so.Notices)
            .WithOne(n => n.SubjectOffering)
            .HasForeignKey(n => n.SubjectOfferingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
