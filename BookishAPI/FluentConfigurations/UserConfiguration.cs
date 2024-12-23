using BookishAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Password)
            .HasMaxLength(255);

        // Navigation properties
        builder.HasMany(u => u.Collections)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Goals)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.SpacedRepetitionGroups)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.VerificationCodes)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.Settings)
            .WithOne(u => u.User)
            .HasForeignKey<UserSettings>("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.NoteTypes)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.NoteCollections)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(u => u.QuoteCollections)
            .WithOne(u => u.User)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}