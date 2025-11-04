namespace BookishAPI;

public static class BookRecommendationMapper
{
    private static Dictionary<string, string> BookNameMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Atomic Habits – James Clear", "Atomic Habits" },
        { "Sapiens – Yuval Noah Harari", "Sapiens" },
        { "1984 – George Orwell", "1984" },
        { "The Alchemist – Paulo Coelho", "The Alchemist" },
        { "The Power of Now – Eckhart Tolle", "The Power of Now" },
        { "Thinking, Fast and Slow – Kahneman", "Thinking, Fast and Slow" },
        { "The Hobbit – J.R.R. Tolkien", "The Hobbit" },
        { "To Kill a Mockingbird – Harper Lee", "To Kill a Mockingbird" },
        { "Becoming – Michelle Obama", "Becoming" },
        { "Quiet – Susan Cain", "Quiet" },
        { "Deep Work – Cal Newport", "Deep Work" },
        { "Man's Search for Meaning – Frankl", "Man's Search for Meaning" },
        { "Steve Jobs – Walter Isaacson", "Steve Jobs" },
        { "Can't Hurt Me – David Goggins", "Can't Hurt Me" },
        { "Educated – Tara Westover", "Educated" },
        { "The 4-Hour Workweek – Tim Ferriss", "The 4-Hour Workweek" },
        { "Dune – Frank Herbert", "Dune" },
        { "Outliers – Malcolm Gladwell", "Outliers" },
        { "The Subtle Art Of Not Giving A Fuck – Manson", "The Subtle Art..." },
        { "The Art of War – Sun Tzu", "The Art of War" },
        { "The Midnight Library – Haig", "The Midnight Library" },
        { "The Artist's Way – Julia Cameron", "The Artist's Way" },
        { "Why We Sleep – Matthew Walker", "Why We Sleep" },
        { "Grit – Angela Duckworth", "Grit" },
        { "The Lean Startup – Eric Ries", "The Lean Startup" },
        { "The Body Keeps the Score – van der Kolk", "The Body Keeps the Score" },
        { "The War of Art – Steven Pressfield", "The War of Art" },
        { "Meditations – Marcus Aurelius", "Meditations" },
        { "Ikigai – Garcia & Miralles", "Ikigai" },
        { "The Road – Cormac McCarthy", "The Road" }
    };

    private static Dictionary<string, int> BookToId = new (StringComparer.OrdinalIgnoreCase)
    {
        { "Atomic Habits", 1 },
        { "Sapiens", 2 },
        { "1984", 3 },
        { "The Alchemist", 4 },
        { "The Power of Now", 5 },
        { "Thinking, Fast and Slow", 6 },
        { "The Hobbit", 7 },
        { "To Kill a Mockingbird", 8 },
        { "Becoming", 9 },
        { "Quiet", 10 },
        { "Deep Work", 11 },
        { "Man's Search for Meaning", 12 },
        { "Steve Jobs", 13 },
        { "Can't Hurt Me", 14 },
        { "Educated", 15 },
        { "The 4-Hour Workweek", 16 },
        { "Dune", 17 },
        { "Outliers", 18 },
        { "The Subtle Art...", 19 },
        { "The Art of War", 20 },
        { "The Midnight Library", 21 },
        { "The Artist's Way", 22 },
        { "Why We Sleep", 23 },
        { "Grit", 24 },
        { "The Lean Startup", 25 },
        { "The Body Keeps the Score", 26 },
        { "The War of Art", 27 },
        { "Meditations", 28 },
        { "Ikigai", 29 },
        { "The Road", 30 }
    };

    public static int? GetBookIdByName(string bookName)
    {
        return BookToId.TryGetValue(bookName, out var id) ? id : null;
    }

    public static string GetBookNameForMatrix(string fullBookName)
    {
        return BookNameMap.TryGetValue(fullBookName, out var matrixName) 
            ? matrixName 
            : fullBookName.Split('–')[0].Trim();
    }
}