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
    
public async Task SeedSelectedBooks()
{
    if (await _context.SelectedBooks.AnyAsync())
    {
        return;
    }

    var selectedBooks = new List<SelectedBook>
    {
        new SelectedBook { Id = 1, Name = "Atomic Habits – James Clear", ImageUrl = "https://m.media-amazon.com/images/I/81ANaVZk5LL._SL1500_.jpg" },
        new SelectedBook { Id = 2, Name = "Sapiens – Yuval Noah Harari", ImageUrl = "https://m.media-amazon.com/images/I/81DOTIO7J6L._SL1500_.jpg" },
        new SelectedBook { Id = 3, Name = "1984 – George Orwell", ImageUrl = "https://m.media-amazon.com/images/I/7180qjGSgDL._SY425_.jpg" },
        new SelectedBook { Id = 4, Name = "The Alchemist – Paulo Coelho", ImageUrl = "https://m.media-amazon.com/images/I/71+2-t7M35L._SL1500_.jpg" },
        new SelectedBook { Id = 5, Name = "The Power of Now – Eckhart Tolle", ImageUrl = "https://m.media-amazon.com/images/I/91u60S7lY7L._SL1500_.jpg" },
        new SelectedBook { Id = 6, Name = "Thinking, Fast and Slow – Kahneman", ImageUrl = "https://m.media-amazon.com/images/I/61fdrEuPJwL._SL1500_.jpg" },
        new SelectedBook { Id = 7, Name = "The Hobbit – J.R.R. Tolkien", ImageUrl = "https://m.media-amazon.com/images/I/71V2v2GtAtL._SL1500_.jpg" },
        new SelectedBook { Id = 8, Name = "To Kill a Mockingbird – Harper Lee", ImageUrl = "https://m.media-amazon.com/images/I/81O7u0dGaWL._SL1500_.jpg" },
        new SelectedBook { Id = 9, Name = "Becoming – Michelle Obama", ImageUrl = "https://m.media-amazon.com/images/I/81cJTmFpG-L._SL1500_.jpg" },
        new SelectedBook { Id = 10, Name = "Quiet – Susan Cain", ImageUrl = "https://m.media-amazon.com/images/I/710KQAE6d5L._SL1500_.jpg" },
        new SelectedBook { Id = 11, Name = "Deep Work – Cal Newport", ImageUrl = "https://m.media-amazon.com/images/I/71pqZChaJkL._SL1500_.jpg" },
        new SelectedBook { Id = 12, Name = "Man's Search for Meaning – Frankl", ImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1535419394i/4069.jpg" },
        new SelectedBook { Id = 13, Name = "Steve Jobs – Walter Isaacson", ImageUrl = "https://m.media-amazon.com/images/I/71sVQDj0SCL._SL1500_.jpg" },
        new SelectedBook { Id = 14, Name = "Can't Hurt Me – David Goggins", ImageUrl = "https://m.media-amazon.com/images/I/81YJFNc54lL._SL1500_.jpg" },
        new SelectedBook { Id = 15, Name = "Educated – Tara Westover", ImageUrl = "https://m.media-amazon.com/images/I/71-4MkLN5jL._SL1500_.jpg" },
        new SelectedBook { Id = 16, Name = "The 4-Hour Workweek – Tim Ferriss", ImageUrl = "https://m.media-amazon.com/images/I/71Pl2BCITWL._SL1500_.jpg" },
        new SelectedBook { Id = 17, Name = "Dune – Frank Herbert", ImageUrl = "https://m.media-amazon.com/images/I/71oO1E-XPuL._SL1500_.jpg" },
        new SelectedBook { Id = 18, Name = "Outliers – Malcolm Gladwell", ImageUrl = "https://m.media-amazon.com/images/I/61sDFu75vAS._SL1500_.jpg" },
        new SelectedBook { Id = 19, Name = "The Subtle Art Of Not Giving A Fuck – Manson", ImageUrl = "https://m.media-amazon.com/images/I/71QKQ9mwV7L._SL1500_.jpg" },
        new SelectedBook { Id = 20, Name = "The Art of War – Sun Tzu", ImageUrl = "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1630683326i/10534.jpg" },
        new SelectedBook { Id = 21, Name = "The Midnight Library – Haig", ImageUrl = "https://m.media-amazon.com/images/I/71FsIkGF3pL._SL1500_.jpg" },
        new SelectedBook { Id = 22, Name = "The Artist's Way – Julia Cameron", ImageUrl = "https://m.media-amazon.com/images/I/918eiy2HZ7L._SL1500_.jpg" },
        new SelectedBook { Id = 23, Name = "Why We Sleep – Matthew Walker", ImageUrl = "https://m.media-amazon.com/images/I/81naK8U4hiL._SL1500_.jpg" },
        new SelectedBook { Id = 24, Name = "Grit – Angela Duckworth", ImageUrl = "https://www.penguinrandomhouse.co.za/sites/penguinbooks.co.za/files/styles/jacket-large/public/cover/9781785040207%20-%20Grit.jpg?itok=pzkTREi2" },
        new SelectedBook { Id = 25, Name = "The Lean Startup – Eric Ries", ImageUrl = "https://m.media-amazon.com/images/I/71sxTeZIi6L._SL1500_.jpg" },
        new SelectedBook { Id = 26, Name = "The Body Keeps the Score – van der Kolk", ImageUrl = "https://m.media-amazon.com/images/I/71Ha3OShqSL._SL1500_.jpg" },
        new SelectedBook { Id = 27, Name = "The War of Art – Steven Pressfield", ImageUrl = "https://m.media-amazon.com/images/I/51lmpnWEuEL._SL1360_.jpg" },
        new SelectedBook { Id = 28, Name = "Meditations – Marcus Aurelius", ImageUrl = "https://m.media-amazon.com/images/I/71wSz6VVk6L._SL1500_.jpg" },
        new SelectedBook { Id = 29, Name = "Ikigai – Garcia & Miralles", ImageUrl = "https://m.media-amazon.com/images/I/71lJBs5MNlL._SL1500_.jpg" },
        new SelectedBook { Id = 30, Name = "The Road – Cormac McCarthy", ImageUrl = "https://m.media-amazon.com/images/I/91bwHfPx-SL._SL1500_.jpg" }
    };

    await _context.SelectedBooks.AddRangeAsync(selectedBooks);
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
            await seeder.SeedSelectedBooks();
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