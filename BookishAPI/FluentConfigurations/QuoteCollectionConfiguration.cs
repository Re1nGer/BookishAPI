using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class QuoteCollectionConfiguration : IEntityTypeConfiguration<QuoteCollection>
{
    public void Configure(EntityTypeBuilder<QuoteCollection> builder)
    {
        builder.HasOne(item => item.User)
            .WithMany(item => item.QuoteCollections)
            .HasForeignKey(item => item.UserId);
    }
}