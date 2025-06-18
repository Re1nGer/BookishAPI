using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder
            .HasMany(item => item.Quotes)
            .WithOne(item => item.Book);

        builder.HasOne(item => item.User)
            .WithMany(item => item.Books);

        builder.HasMany(item => item.Notes)
            .WithOne(item => item.Book);
        
        builder.HasOne(item => item.ReadEvent)
            .WithOne(item => item.Book);

        builder
            .HasMany(item => item.ReadingSessions)
            .WithOne(item => item.Book);

        builder.HasMany(item => item.Genres)
            .WithMany(item => item.Books)
            .UsingEntity(j => j.ToTable("BookGenres"));

        builder.HasMany(item => item.BookCollections)
            .WithMany(item => item.Books)
            .UsingEntity(j => j.ToTable("CollectionBooks"));
    }
}