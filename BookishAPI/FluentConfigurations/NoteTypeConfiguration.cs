using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class NoteTypeConfiguration : IEntityTypeConfiguration<NoteType>
{
    public void Configure(EntityTypeBuilder<NoteType> builder)
    {
        builder.HasOne(item => item.User)
            .WithMany(item => item.NoteTypes)
            .HasForeignKey(item => item.UserId);
    }
}