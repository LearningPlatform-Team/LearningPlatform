using LearningPlatform.Domain.Entities.Engagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningPlatform.Infrastructure.Configurations.Engagement;

public class EventReminderConfiguration : IEntityTypeConfiguration<EventReminder>
{
    public void Configure(EntityTypeBuilder<EventReminder> builder)
    {
        builder.HasKey(er => new { er.EventId, er.UserId });

        builder.Property(er => er.MinutesBefore)
            .IsRequired();

        builder.HasIndex(er => er.UserId);

        // Relationships
        builder.HasOne(er => er.User)
            .WithMany(u => u.EventReminders)
            .HasForeignKey(er => er.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(er => er.Event)
            .WithMany()
            .HasForeignKey(er => er.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
