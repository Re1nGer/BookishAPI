using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Status).HasMaxLength(20);
        builder.HasIndex(e => new { e.UserId, e.GroupId, e.SentAt });
    }
}