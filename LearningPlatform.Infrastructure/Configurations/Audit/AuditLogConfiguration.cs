using LearningPlatform.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Audit;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(al => al.Id);

        builder.Property(al => al.EntityType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(al => al.EntityId)
            .IsRequired();

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.OldValues)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(al => al.NewValues)
            .HasColumnType("NVARCHAR(MAX)");

        builder.Property(al => al.ChangedAt)
            .IsRequired();

        builder.HasIndex(al => al.EntityType);

        builder.HasIndex(al => al.EntityId);

        builder.HasIndex(al => al.ChangedAt);

        builder.HasIndex(al => new { al.EntityType, al.EntityId });

        // Relationships
        builder.HasOne(al => al.ChangedBy)
            .WithMany()
            .HasForeignKey(al => al.ChangedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
