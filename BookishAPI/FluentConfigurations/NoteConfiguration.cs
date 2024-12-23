using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasMany(item => item.NoteCollections)
            .WithMany(item => item.Notes);

        builder.HasMany(item => item.NoteImages)
            .WithOne(item => item.Note)
            .HasForeignKey(item => item.NoteId);
    }
}