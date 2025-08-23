using Microsoft.EntityFrameworkCore;

namespace BookishAPI;

public class Seeder
{
    private readonly BookAppContext _context;

    public Seeder(BookAppContext context)
    {
        _context = context;
    }

    public async Task SeedInterestAreas()
    {
        // Check if data already exists
        if (await _context.InterestAreas.AnyAsync())
        {
            return; // Data already seeded
        }
        
        var bookInterests = new List<InterestArea>
        {
            new InterestArea { Id = 1, Name = "Fiction" },
            new InterestArea { Id = 2, Name = "Non-Fiction" },
            new InterestArea { Id = 3, Name = "Fantasy" },
            new InterestArea { Id = 4, Name = "Mystery" },
            new InterestArea { Id = 5, Name = "Detective" },
            new InterestArea { Id = 6, Name = "Romance" },
            new InterestArea { Id = 7, Name = "Biography" },
            new InterestArea { Id = 8, Name = "History" },
            new InterestArea { Id = 9, Name = "Psychology" },
            new InterestArea { Id = 10, Name = "Philosophy" },
            new InterestArea { Id = 11, Name = "Science" },
            new InterestArea { Id = 12, Name = "Biology" },
            new InterestArea { Id = 13, Name = "Nature" },
            new InterestArea { Id = 14, Name = "Technology" },
            new InterestArea { Id = 15, Name = "Art" },
            new InterestArea { Id = 16, Name = "Creativity" },
            new InterestArea { Id = 17, Name = "Habits" },
            new InterestArea { Id = 18, Name = "Productivity" },
            new InterestArea { Id = 19, Name = "Business" },
            new InterestArea { Id = 20, Name = "Health & Fitness" },
            new InterestArea { Id = 21, Name = "Spirituality" },
            new InterestArea { Id = 22, Name = "Politics" },
            new InterestArea { Id = 23, Name = "Memoir" },
            new InterestArea { Id = 24, Name = "Self-Help" },
            new InterestArea { Id = 25, Name = "Education" }
        };
        
        await _context.InterestAreas.AddRangeAsync(bookInterests);
        await _context.SaveChangesAsync();
    }
    
    public async Task SeedSelectedBooks()
    {
        // Check if data already exists
        if (await _context.SelectedBooks.AnyAsync())
        {
            return; // Data already seeded
        }
        //need image urls
    }

    public async Task SeedReadingPurposes()
    {
        if (await _context.ReadingPurposes.AnyAsync())
        {
            return;
        }
        
        var readingPurposes = new List<ReadingPurpose>
        {
            new ReadingPurpose { Id = 1, Name = "Personal Growth & Self-Improvement" },
            new ReadingPurpose { Id = 2, Name = "Social & Connection" },
            new ReadingPurpose { Id = 3, Name = "Creativity & Imagination" },
            new ReadingPurpose { Id = 4, Name = "Professional Development" },
            new ReadingPurpose { Id = 5, Name = "Inspiration & Motivation" },
            new ReadingPurpose { Id = 6, Name = "Academic & Educational Purposes" },
            new ReadingPurpose { Id = 7, Name = "Healthy Lifestyle" }
        };
        
        await _context.ReadingPurposes.AddRangeAsync(readingPurposes);
        await _context.SaveChangesAsync();
    }
}

public static class SeederExtensions
{
    public static async Task<IHost> SeedDataAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<BookAppContext>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Run seeders
            var seeder = new Seeder(context);
            
            await seeder.SeedInterestAreas();
            await seeder.SeedReadingPurposes();
        }
        catch (Exception ex)
        {
            // Log the error (you might want to use ILogger here)
            Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            throw;
        }
        
        return host;
    }
}