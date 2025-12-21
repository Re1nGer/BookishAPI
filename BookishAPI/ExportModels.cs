namespace BookishAPI;


// Export DTOs
public class ExportedUserData
{
    public ExportedProfile Profile { get; set; }
    public ExportedSettings? Settings { get; set; }
    public ExportedStreak? Streak { get; set; }
    public List<string> InterestAreas { get; set; } = new();
    public List<string> ReadingPurposes { get; set; } = new();
    public List<ExportedSelectedBook> SelectedBooks { get; set; } = new();
    public List<ExportedBook> Books { get; set; } = new();
    public List<ExportedBookCollection> BookCollections { get; set; } = new();
    public List<ExportedNoteCollection> NoteCollections { get; set; } = new();
    public List<ExportedQuoteCollection> QuoteCollections { get; set; } = new();
    public List<ExportedSpacedRepetitionGroup> SpacedRepetitionGroups { get; set; } = new();
    public List<ExportedGoal> Goals { get; set; } = new();
    public List<ExportedReadEvent> ReadEvents { get; set; } = new();
    public List<ExportedReadStat> ReadStats { get; set; } = new();
    public ExportMetadata ExportMetadata { get; set; }
}

public class ExportedProfile
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsPremiumUser { get; set; }
    public bool IsNotificationsEnabled { get; set; }
    public string? TimeZoneId { get; set; }
    public TimeOnly DailyReminderAt { get; set; }
    public int? BookAmountGoalInYear { get; set; }
    public int? PagesReadGoalInYear { get; set; }
    public bool HasCompletedOnboarding { get; set; }
}

public class ExportedSettings
{
    public string TimeFormat { get; set; }
}

public class ExportedStreak
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateOnly? LastActivityDate { get; set; }
    public int MinutesReadToday { get; set; }
    public int PagesReadToday { get; set; }
}

public class ExportedSelectedBook
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}

public class ExportedBook
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Status { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? FinalThoughts { get; set; }
    public List<string> Genres { get; set; } = new();
    public List<ExportedNote> Notes { get; set; } = new();
    public List<ExportedQuote> Quotes { get; set; } = new();
    public List<ExportedReadingSession> ReadingSessions { get; set; } = new();
}

public class ExportedNote
{
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TypeName { get; set; }
    public List<string> Images { get; set; } = new();
}

public class ExportedQuote
{
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ExportedReadingSession
{
    public DateTime EndTime { get; set; }
    public int EndPage { get; set; }
    public int PagesRead { get; set; }
    public int DurationInSeconds { get; set; }
}

public class ExportedBookCollection
{
    public string Name { get; set; }
    public List<string> BookTitles { get; set; } = new();
}

public class ExportedNoteCollection
{
    public string Name { get; set; }
    public List<ExportedNoteReference> Notes { get; set; } = new();
}

public class ExportedNoteReference
{
    public string BookTitle { get; set; }
    public string Content { get; set; }
}

public class ExportedQuoteCollection
{
    public string Name { get; set; }
    public List<ExportedQuoteReference> Quotes { get; set; } = new();
}

public class ExportedQuoteReference
{
    public string BookTitle { get; set; }
    public string Content { get; set; }
}

public class ExportedSpacedRepetitionGroup
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public string? Mode { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Notes { get; set; } = new();
    public List<string> Quotes { get; set; } = new();
}

public class ExportedGoal
{
    public int Year { get; set; }
    public string Type { get; set; }
    public string Period { get; set; }
    public int Target { get; set; }
    public int TargetBooks { get; set; }
    public int CompletedBooks { get; set; }
}

public class ExportedReadEvent
{
    public string BookTitle { get; set; }
    public DateTime CreatedAt { get; set; }
    public short Rating { get; set; }
    public string? Memo { get; set; }
}

public class ExportedReadStat
{
    public string BookTitle { get; set; }
    public DateTime ReadAt { get; set; }
    public int PageNumber { get; set; }
}

public class ExportMetadata
{
    public DateTime ExportedAt { get; set; }
    public string AppName { get; set; }
    public string Version { get; set; }
}