using Microsoft.EntityFrameworkCore;

// Entities

namespace BookishAPI;



public class VerificationCode
{
    public int Id { get; set; }
    public string Code { get; set; } 
    public DateTime CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public bool IsUsed { get; set; }
    public User User { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? Password { get; set; }
    public List<NoteType> NoteTypes { get; set; }
    public List<NoteCollection> NoteCollections { get; set; }
    public List<QuoteCollection> QuoteCollections { get; set; }
    public List<Book> Books { get; set; }
    public List<BookCollection> Collections { get; set; }
    public List<Goal> Goals { get; set; }
    public List<SpacedRepetitionGroup> SpacedRepetitionGroups { get; set; }
    public List<VerificationCode> VerificationCodes { get; set; }
    public UserSettings Settings { get; set; }
    public List<ReadEvent> ReadEvents { get; set; }
}

public class BookCollection
{
    public int Id { get; set; }
    public string Name { get; set; }
    //icon id that's randomly assigned here 1-39
    //then this id is mapped on mobile client to svg icon
    public int IconId { get; set; } 
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Book> Books { get; set; }
}

public class NoteCollection
{
    public int Id { get; set; }
    public string Name { get; set; }
    //icon id that's randomly assigned here 1-39
    //then this id is mapped on mobile client to svg icon
    public int IconId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Note> Notes { get; set; }
}

public class QuoteCollection
{
    public int Id { get; set; }
    public string Name { get; set; }
    //icon id that's randomly assigned here 1-39
    //then this id is mapped on mobile client to svg icon
    public int IconId { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Quote> Quotes { get; set; }
}

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Description { get; set; }
    public string Author { get; set; }
    public string ImageUrl { get; set; }
    public string? FinalThoughts { get; set; }
    public string? FinalThoughtsImage { get; set; }
    public BookStatus Status { get; set; }
    public List<BookCollection> BookCollections { get; set; }
    public List<Genre> Genres { get; set; }
    public List<Note> Notes { get; set; }
    public List<Quote> Quotes { get; set; }
    public List<ReadingSession> ReadingSessions { get; set; }
    public ReadEvent ReadEvent { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Book> Books { get; set; }
}

public class Note
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int BookId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Book Book { get; set; }
    public NoteType Type { get; set; }
    public Quote? RelatedQuote { get; set; }
    public List<SpacedRepetitionGroup> SpacedRepetitionGroups { get; set; }
    public List<NoteCollection> NoteCollections { get; set; }
    public List<NoteImage> NoteImages { get; set; }
}

public class NoteImage
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ImageUrl { get; set; }
    public Note Note { get; set; }
    public int NoteId { get; set; }
}

public class NoteType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}

public class Quote
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public List<Note> RelatedNotes { get; set; }
    public List<SpacedRepetitionGroup> SpacedRepetitionGroups { get; set; }
    public List<QuoteCollection> QuoteCollections { get; set; }
}

public class Goal
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public int TargetBooks { get; set; }
    public int CompletedBooks { get; set; }
    public int Year { get; set; }
    public GoalType Type { get; set; }
    public GoalPeriod Period { get; set; }
    public int Target { get; set; }
}

public class SpacedRepetitionGroup
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public int IconId { get; set; }
    public List<Note> Notes { get; set; }
    public List<Quote> Quotes { get; set; }
    public DateTime RemindAt { get; set; }
}

public class ReadingSession
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int StartPage { get; set; }
    public int EndPage { get; set; }
    public TimeSpan Duration { get; set; }
    public SessionStatus Status { get; set; }
}

public enum SessionStatus
{
    InProgress,
    Completed,
    Paused
}

public enum BookStatus
{
    ToRead,
    Reading,
    Finished,
    GaveUp,
    Paused
}

public enum TimeFormat
{
    Format12Hour,
    Format24Hour
}

public enum GoalType
{
    PagesRead,
    TimeSpentReading
}

public enum GoalPeriod
{
    Daily,
    Yearly
}

public class UserSettings
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool DailyReminderEnabled { get; set; }
    public TimeSpan DailyReminderTime { get; set; }
    public TimeFormat TimeFormat { get; set; }
}

public class ReadEvent
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public Book Book { get; set; }
    public int BookId { get; set; }
    public short Rating { get; set; }
    public string? PhotoId { get; set; } //Id of the image associated with the event
    public string? Memo { get; set; } //short memo associated with the event
}


// DbContext

public class BookAppContext : DbContext
{
    public BookAppContext(DbContextOptions<BookAppContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<BookCollection> BookCollections { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<NoteType> NoteTypes { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<SpacedRepetitionGroup> SpacedRepetitionGroups { get; set; }
    public DbSet<ReadingSession> ReadingSessions { get; set; }
    public DbSet<UserSettings> UserSettings { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<NoteCollection> NoteCollections { get; set; }
    public DbSet<QuoteCollection> QuoteCollections { get; set; }
    public DbSet<ReadEvent> ReadEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookAppContext).Assembly);
    }
}
