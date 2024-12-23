using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class NoteCollectionConfiguration : IEntityTypeConfiguration<NoteCollection>
{
    public void Configure(EntityTypeBuilder<NoteCollection> builder)
    {
        builder.HasOne(item => item.User)
            .WithMany(item => item.NoteCollections)
            .HasForeignKey(item => item.UserId);
    }
}