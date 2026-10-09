using LearningPlatform.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Academic;

public class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(sp => sp.UserId);

        builder.Property(sp => sp.StudentCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(sp => sp.StudentCode)
            .IsUnique();

        // Relationships are configured from the User configuration
        builder.HasMany(sp => sp.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
