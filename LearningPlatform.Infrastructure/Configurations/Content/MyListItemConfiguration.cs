using LearningPlatform.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Content;

public class MyListItemConfiguration : IEntityTypeConfiguration<MyListItem>
{
    public void Configure(EntityTypeBuilder<MyListItem> builder)
    {
        builder.HasKey(mli => new { mli.UserId, mli.LectureId });

        builder.HasIndex(mli => mli.UserId);

        // Relationships
        builder.HasOne(mli => mli.User)
            .WithMany(u => u.MyListItems)
            .HasForeignKey(mli => mli.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mli => mli.Lecture)
            .WithMany(l => l.MyListItems)
            .HasForeignKey(mli => mli.LectureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
