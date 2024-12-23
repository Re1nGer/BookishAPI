using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasMany(item => item.RelatedNotes)
            .WithOne(item => item.RelatedQuote);

        builder.HasMany(item => item.QuoteCollections)
            .WithMany(item => item.Quotes);
    }
}