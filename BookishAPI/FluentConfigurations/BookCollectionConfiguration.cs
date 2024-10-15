using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class BookCollectionConfiguration : IEntityTypeConfiguration<BookCollection>
{
    public void Configure(EntityTypeBuilder<BookCollection> builder)
    {
        builder.HasOne(item => item.User)
            .WithMany(item => item.Collections)
            .HasForeignKey(item => item.UserId);
    }
}