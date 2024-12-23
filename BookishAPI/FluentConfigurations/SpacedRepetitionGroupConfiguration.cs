using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class SpacedRepetitionGroupConfiguration : IEntityTypeConfiguration<SpacedRepetitionGroup>
{
    public void Configure(EntityTypeBuilder<SpacedRepetitionGroup> builder)
    {
        builder.HasMany(item => item.Quotes)
            .WithMany(item => item.SpacedRepetitionGroups);

        builder.HasMany(item => item.Notes)
            .WithMany(item => item.SpacedRepetitionGroups);

    }
}