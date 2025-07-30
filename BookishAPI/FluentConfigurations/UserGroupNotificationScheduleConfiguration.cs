using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class UserGroupNotificationScheduleConfiguration : IEntityTypeConfiguration<UserGroupNotificationSchedule>
{
    public void Configure(EntityTypeBuilder<UserGroupNotificationSchedule> entity)
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.ScheduledTime);
            
        // Foreign key to SpacedRepetitionGroup
        entity.HasOne(e => e.Group)
            .WithMany(e => e.Schedules)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}