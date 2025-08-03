using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class ReadStatConfiguration : IEntityTypeConfiguration<ReadStat>
{
    public void Configure(EntityTypeBuilder<ReadStat> builder)
    {
        builder.HasOne(j => j.Book)
            .WithMany(j => j.ReadStats)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(j => j.User)
            .WithMany(j => j.ReadStats)
            .OnDelete(DeleteBehavior.Cascade);
    }
}