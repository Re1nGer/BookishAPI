using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class SpacedRepetitionGroupConfiguration : IEntityTypeConfiguration<SpacedRepetitionGroup>
{
    public void Configure(EntityTypeBuilder<SpacedRepetitionGroup> builder)
    {
        builder.HasMany(item => item.Quotes)
            .WithMany(item => item.SpacedRepetitionGroups)
            .UsingEntity(j => j.ToTable("QuotesSpacedRepetition"));
        
        builder.HasMany(item => item.Notes)
            .WithMany(item => item.SpacedRepetitionGroups)
            .UsingEntity(j => j.ToTable("NotesSpacedRepetition"));
        
    }
}