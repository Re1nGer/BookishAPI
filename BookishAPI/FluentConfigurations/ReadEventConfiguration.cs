using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookishAPI.FluentConfigurations;

public class ReadEventConfiguration : IEntityTypeConfiguration<ReadEvent>
{
    public void Configure(EntityTypeBuilder<ReadEvent> builder)
    {
        builder.Property(j => j.PhotoId)
            .IsRequired(false);
        
        builder.Property(j => j.Memo)
            .IsRequired(false);
    }
}