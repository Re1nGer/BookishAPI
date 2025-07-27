using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharpCompress.Compressors.Xz;

namespace BookishAPI.FluentConfigurations;

public class UserPushTokenConfiguration : IEntityTypeConfiguration<UserPushToken>
{
    public void Configure(EntityTypeBuilder<UserPushToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.DeviceToken).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Platform).HasMaxLength(20).IsRequired();
        builder.HasIndex(e => new { e.UserId, e.Platform });
        builder.HasIndex(e => e.DeviceToken);
    }
}