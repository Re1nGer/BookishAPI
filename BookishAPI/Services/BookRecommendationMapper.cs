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

    //TODO: To extract out into separate table in db
    public static List<CollectionData> GetAllPurposeCollections()
    {
        return new List<CollectionData>
        {
            // Dopamine Detox Guide - Personal Growth (1), Professional Development (4), Healthy Lifestyle (7)
            new CollectionData
            {
                PurposeIds = new[] { 1, 4, 7 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Atomic Habits – James Clear",
                        ImageUrl = "https://m.media-amazon.com/images/I/81ANaVZk5LL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Deep Work – Cal Newport",
                        ImageUrl = "https://m.media-amazon.com/images/I/71pqZChaJkL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Digital Minimalism – Cal Newport", ImageUrl = "" },
                    new BookInfo { Name = "Indistractable – Nir Eyal", ImageUrl = "" },
                    new BookInfo { Name = "Essentialism – Greg McKeown", ImageUrl = "" },
                    new BookInfo { Name = "The Power of Habit – Charles Duhigg", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The 4-Hour Workweek – Tim Ferriss",
                        ImageUrl = "https://m.media-amazon.com/images/I/71Pl2BCITWL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Hyperfocus – Chris Bailey", ImageUrl = "" },
                    new BookInfo { Name = "Make Time – Knapp & Zeratsky", ImageUrl = "" },
                    new BookInfo { Name = "Getting Things Done – David Allen", ImageUrl = "" }
                }
            },
            // Top 10 to Know History Inside Out - Academic (6), Professional Development (4)
            new CollectionData
            {
                PurposeIds = new[] { 6, 4 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Sapiens – Yuval Noah Harari",
                        ImageUrl = "https://m.media-amazon.com/images/I/81DOTIO7J6L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Guns, Germs, and Steel – Jared Diamond", ImageUrl = "" },
                    new BookInfo { Name = "Why Nations Fail – Acemoglu & Robinson", ImageUrl = "" },
                    new BookInfo { Name = "The Prince – Niccolò Machiavelli", ImageUrl = "" },
                    new BookInfo { Name = "A People's History of the United States – Howard Zinn", ImageUrl = "" },
                    new BookInfo { Name = "A Brief History of Time – Stephen Hawking", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Art of War – Sun Tzu",
                        ImageUrl =
                            "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1630683326i/10534.jpg"
                    },
                    new BookInfo { Name = "The Diary of a Young Girl – Anne Frank", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "Man's Search for Meaning – Viktor Frankl",
                        ImageUrl =
                            "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1535419394i/4069.jpg"
                    },
                    new BookInfo { Name = "The Righteous Mind – Jonathan Haidt", ImageUrl = "" }
                }
            },
            // The Mental Fitness Stack - Personal Growth (1), Professional Development (4)
            new CollectionData
            {
                PurposeIds = new[] { 1, 4 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Thinking, Fast and Slow – Kahneman",
                        ImageUrl = "https://m.media-amazon.com/images/I/61fdrEuPJwL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Atomic Habits – James Clear",
                        ImageUrl = "https://m.media-amazon.com/images/I/81ANaVZk5LL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Deep Work – Cal Newport",
                        ImageUrl = "https://m.media-amazon.com/images/I/71pqZChaJkL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Can't Hurt Me – David Goggins",
                        ImageUrl = "https://m.media-amazon.com/images/I/81YJFNc54lL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Mindset – Carol Dweck", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Subtle Art Of Not Giving A Fuck – Mark Manson",
                        ImageUrl = "https://m.media-amazon.com/images/I/71QKQ9mwV7L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Make Time – Knapp & Zeratsky", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Power of Now – Eckhart Tolle",
                        ImageUrl = "https://m.media-amazon.com/images/I/91u60S7lY7L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Drive – Daniel Pink", ImageUrl = "" },
                    new BookInfo { Name = "The 7 Habits of Highly Effective People – Stephen Covey", ImageUrl = "" }
                }
            },
            // Creative Mind Unlocked - Creativity & Imagination (3), Personal Growth (1)
            new CollectionData
            {
                PurposeIds = new[] { 3, 1 },
                Books = new[]
                {
                    new BookInfo { Name = "Steal Like an Artist – Austin Kleon", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The War of Art – Steven Pressfield",
                        ImageUrl = "https://m.media-amazon.com/images/I/51lmpnWEuEL._SL1360_.jpg"
                    },
                    new BookInfo { Name = "Big Magic – Elizabeth Gilbert", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Artist's Way – Julia Cameron",
                        ImageUrl = "https://m.media-amazon.com/images/I/918eiy2HZ7L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Flow – Mihaly Csikszentmihalyi", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Power of Now – Eckhart Tolle",
                        ImageUrl = "https://m.media-amazon.com/images/I/91u60S7lY7L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Letters to a Young Poet – Rainer Maria Rilke", ImageUrl = "" },
                    new BookInfo { Name = "Ways of Seeing – John Berger", ImageUrl = "" },
                    new BookInfo { Name = "On Writing – Stephen King", ImageUrl = "" },
                    new BookInfo { Name = "Show Your Work – Austin Kleon", ImageUrl = "" }
                }
            },
            // Imaginative Realms & Escapes + Mystery & Page-turners + Epic Journeys - Entertainment and Relaxation, Creativity & Imagination (3)
            new CollectionData
            {
                PurposeIds = new[] { 3 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Dune – Frank Herbert",
                        ImageUrl = "https://m.media-amazon.com/images/I/71oO1E-XPuL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "The Hobbit – J.R.R. Tolkien",
                        ImageUrl = "https://m.media-amazon.com/images/I/71V2v2GtAtL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "The Name of the Wind – Patrick Rothfuss", ImageUrl = "" },
                    new BookInfo { Name = "Ender's Game – Orson Scott Card", ImageUrl = "" },
                    new BookInfo { Name = "Brave New World – Aldous Huxley", ImageUrl = "" },
                    new BookInfo { Name = "Fahrenheit 451 – Ray Bradbury", ImageUrl = "" },
                    new BookInfo { Name = "The Left Hand of Darkness – Ursula K. Le Guin", ImageUrl = "" },
                    new BookInfo { Name = "The Martian – Andy Weir", ImageUrl = "" },
                    new BookInfo { Name = "Good Omens – Terry Pratchett & Neil Gaiman", ImageUrl = "" },
                    new BookInfo { Name = "Murder on the Orient Express – Agatha Christie", ImageUrl = "" },
                    new BookInfo { Name = "Gone Girl – Gillian Flynn", ImageUrl = "" },
                    new BookInfo { Name = "The Girl with the Dragon Tattoo – Stieg Larsson", ImageUrl = "" },
                    new BookInfo { Name = "Sherlock Holmes – Arthur Conan Doyle", ImageUrl = "" },
                    new BookInfo { Name = "The Da Vinci Code – Dan Brown", ImageUrl = "" },
                    new BookInfo { Name = "The Silent Patient – Alex Michaelides", ImageUrl = "" },
                    new BookInfo { Name = "Big Little Lies – Liane Moriarty", ImageUrl = "" },
                    new BookInfo { Name = "Before I Go to Sleep – S.J. Watson", ImageUrl = "" },
                    new BookInfo { Name = "In the Woods – Tana French", ImageUrl = "" },
                    new BookInfo { Name = "And Then There Were None – Agatha Christie", ImageUrl = "" },
                    new BookInfo { Name = "Wild – Cheryl Strayed", ImageUrl = "" },
                    new BookInfo { Name = "Into the Wild – Jon Krakauer", ImageUrl = "" },
                    new BookInfo { Name = "Shantaram – Gregory David Roberts", ImageUrl = "" },
                    new BookInfo { Name = "Eat Pray Love – Elizabeth Gilbert", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "The Road – Cormac McCarthy",
                        ImageUrl = "https://m.media-amazon.com/images/I/91bwHfPx-SL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Station Eleven – Emily St. John Mandel", ImageUrl = "" },
                    new BookInfo { Name = "The Glass Castle – Jeannette Walls", ImageUrl = "" }
                }
            },
            // Stories That Make You Empathize - Social & Connection (2)
            new CollectionData
            {
                PurposeIds = new[] { 2 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "To Kill a Mockingbird – Harper Lee",
                        ImageUrl = "https://m.media-amazon.com/images/I/81O7u0dGaWL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Pride and Prejudice – Jane Austen", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "Educated – Tara Westover",
                        ImageUrl = "https://m.media-amazon.com/images/I/71-4MkLN5jL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "The Catcher in the Rye – J.D. Salinger", ImageUrl = "" },
                    new BookInfo { Name = "The Diary of a Young Girl – Anne Frank", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "Becoming – Michelle Obama",
                        ImageUrl = "https://m.media-amazon.com/images/I/81cJTmFpG-L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "The Fault in Our Stars – John Green", ImageUrl = "" },
                    new BookInfo { Name = "The Book Thief – Markus Zusak", ImageUrl = "" },
                    new BookInfo { Name = "A Man Called Ove – Fredrik Backman", ImageUrl = "" },
                    new BookInfo { Name = "Normal People – Sally Rooney", ImageUrl = "" }
                }
            },
            // Life Lessons from Real Lives - Inspiration & Motivation (5)
            new CollectionData
            {
                PurposeIds = new[] { 5 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Man's Search for Meaning – Viktor Frankl",
                        ImageUrl =
                            "https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1535419394i/4069.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Becoming – Michelle Obama",
                        ImageUrl = "https://m.media-amazon.com/images/I/81cJTmFpG-L._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Educated – Tara Westover",
                        ImageUrl = "https://m.media-amazon.com/images/I/71-4MkLN5jL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Can't Hurt Me – David Goggins",
                        ImageUrl = "https://m.media-amazon.com/images/I/81YJFNc54lL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Steve Jobs – Walter Isaacson",
                        ImageUrl = "https://m.media-amazon.com/images/I/71sVQDj0SCL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Shoe Dog – Phil Knight", ImageUrl = "" },
                    new BookInfo { Name = "The Glass Castle – Jeannette Walls", ImageUrl = "" },
                    new BookInfo { Name = "Born a Crime – Trevor Noah", ImageUrl = "" },
                    new BookInfo { Name = "The Long Walk to Freedom – Nelson Mandela", ImageUrl = "" },
                    new BookInfo { Name = "Eat Pray Love – Elizabeth Gilbert", ImageUrl = "" }
                }
            },
            // The Builder's Toolkit - Professional Development (4)
            new CollectionData
            {
                PurposeIds = new[] { 4 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "The 4-Hour Workweek – Tim Ferriss",
                        ImageUrl = "https://m.media-amazon.com/images/I/71Pl2BCITWL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Deep Work – Cal Newport",
                        ImageUrl = "https://m.media-amazon.com/images/I/71pqZChaJkL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Thinking, Fast and Slow – Kahneman",
                        ImageUrl = "https://m.media-amazon.com/images/I/61fdrEuPJwL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "Drive – Daniel Pink", ImageUrl = "" },
                    new BookInfo { Name = "Essentialism – Greg McKeown", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "Atomic Habits – James Clear",
                        ImageUrl = "https://m.media-amazon.com/images/I/81ANaVZk5LL._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Outliers – Malcolm Gladwell",
                        ImageUrl = "https://m.media-amazon.com/images/I/61sDFu75vAS._SL1500_.jpg"
                    },
                    new BookInfo
                    {
                        Name = "Grit – Angela Duckworth",
                        ImageUrl =
                            "https://www.penguinrandomhouse.co.za/sites/penguinbooks.co.za/files/styles/jacket-large/public/cover/9781785040207%20-%20Grit.jpg?itok=pzkTREi2"
                    },
                    new BookInfo { Name = "Rework – Jason Fried & David Heinemeier Hansson", ImageUrl = "" },
                    new BookInfo { Name = "Start With Why – Simon Sinek", ImageUrl = "" }
                }
            },
            // Scientific Curiosity Pack + Brains and Biases - Academic (6), Personal Growth (1)
            new CollectionData
            {
                PurposeIds = new[] { 6, 1 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "Sapiens – Yuval Noah Harari",
                        ImageUrl = "https://m.media-amazon.com/images/I/81DOTIO7J6L._SL1500_.jpg"
                    },
                    new BookInfo { Name = "A Brief History of Time – Stephen Hawking", ImageUrl = "" },
                    new BookInfo { Name = "The Gene – Siddhartha Mukherjee", ImageUrl = "" },
                    new BookInfo { Name = "The Selfish Gene – Richard Dawkins", ImageUrl = "" },
                    new BookInfo { Name = "The Emperor of All Maladies – Siddhartha Mukherjee", ImageUrl = "" },
                    new BookInfo { Name = "Guns, Germs, and Steel – Jared Diamond", ImageUrl = "" },
                    new BookInfo
                    {
                        Name = "Thinking, Fast and Slow – Kahneman",
                        ImageUrl = "https://m.media-amazon.com/images/I/61fdrEuPJwL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "The Immortal Life of Henrietta Lacks – Rebecca Skloot", ImageUrl = "" },
                    new BookInfo { Name = "Cosmos – Carl Sagan", ImageUrl = "" },
                    new BookInfo { Name = "The Body – Bill Bryson", ImageUrl = "" },
                    new BookInfo { Name = "The Power of Habit – Charles Duhigg", ImageUrl = "" },
                    new BookInfo { Name = "The Paradox of Choice – Barry Schwartz", ImageUrl = "" },
                    new BookInfo { Name = "Blink – Malcolm Gladwell", ImageUrl = "" },
                    new BookInfo { Name = "Mindset – Carol Dweck", ImageUrl = "" },
                    new BookInfo { Name = "Nudge – Richard Thaler & Cass Sunstein", ImageUrl = "" },
                    new BookInfo { Name = "Predictably Irrational – Dan Ariely", ImageUrl = "" },
                    new BookInfo { Name = "The Art of Thinking Clearly – Rolf Dobelli", ImageUrl = "" },
                    new BookInfo { Name = "Influence – Robert Cialdini", ImageUrl = "" }
                }
            },
            // Classics You Shouldn't Miss - Academic (6), Inspiration (5)
            new CollectionData
            {
                PurposeIds = new[] { 6, 5 },
                Books = new[]
                {
                    new BookInfo
                    {
                        Name = "1984 – George Orwell",
                        ImageUrl = "https://prodimage.images-bn.com/pimages/9780452262935_p0_v6_s1200x1200.jpg"
                    },
                    new BookInfo
                    {
                        Name = "To Kill a Mockingbird – Harper Lee",
                        ImageUrl = "https://m.media-amazon.com/images/I/81O7u0dGaWL._SL1500_.jpg"
                    },
                    new BookInfo { Name = "The Great Gatsby – F. Scott Fitzgerald", ImageUrl = "" },
                    new BookInfo { Name = "The Catcher in the Rye – J.D. Salinger", ImageUrl = "" },
                    new BookInfo { Name = "Pride and Prejudice – Jane Austen", ImageUrl = "" },
                    new BookInfo { Name = "Jane Eyre – Charlotte Brontë", ImageUrl = "" },
                    new BookInfo { Name = "Crime and Punishment – Fyodor Dostoevsky", ImageUrl = "" },
                    new BookInfo { Name = "Frankenstein – Mary Shelley", ImageUrl = "" },
                    new BookInfo { Name = "Les Misérables – Victor Hugo", ImageUrl = "" },
                    new BookInfo { Name = "The Brothers Karamazov – Fyodor Dostoevsky", ImageUrl = "" }
                }
            }
        };

    }
    public static Dictionary<string, List<string>> GetCollectionThemes()
    {
        return new Dictionary<string, List<string>>
        {
            // Note: These are the 16 collections from the weight matrix
            // Mapping them to the themed collections you provided

            ["Dopamine Detox Guide"] = new List<string>
            {
                "Atomic Habits", "Deep Work", "Digital Minimalism", "The Power of Now",
                "Why We Sleep", "Can't Hurt Me", "Ikigai", "Essentialism"
            },

            ["The Human Odyssey"] = new List<string>
            {
                "Sapiens", "Guns, Germs, and Steel", "Why Nations Fail", "The Prince",
                "A People's History of the United States", "A Brief History of Time",
                "The Art of War", "The Diary of a Young Girl", "Man's Search for Meaning",
                "The Righteous Mind"
            },

            ["Rebel Thinkers"] = new List<string>
            {
                "1984", "The Art of War", "The Road"
            },

            ["Spiritual Explorers"] = new List<string>
            {
                "The Power of Now", "Man's Search for Meaning", "The Alchemist",
                "Ikigai", "The Midnight Library"
            },

            ["Creativity Unlocked"] = new List<string>
            {
                "Steal Like an Artist", "The War of Art", "Big Magic", "The Artist's Way",
                "Flow", "The Power of Now", "Letters to a Young Poet", "Ways of Seeing",
                "On Writing", "Show Your Work"
            },

            ["Leaders & Builders"] = new List<string>
            {
                "The 4-Hour Workweek", "Deep Work", "Thinking, Fast and Slow", "Drive",
                "Essentialism", "Atomic Habits", "Outliers", "Grit", "Rework", "Start With Why",
                "The Lean Startup", "Steve Jobs"
            },

            ["Mind Gym"] = new List<string>
            {
                "Thinking, Fast and Slow", "Atomic Habits", "Deep Work", "Can't Hurt Me",
                "Mindset", "The Subtle Art...", "Make Time", "The Power of Now", "Drive",
                "The 7 Habits of Highly Effective People", "The Power of Habit",
                "Outliers", "Grit", "Quiet"
            },

            ["Epic Journeys"] = new List<string>
            {
                "Dune", "The Hobbit", "The Name of the Wind", "Ender's Game",
                "Brave New World", "Fahrenheit 451", "The Left Hand of Darkness",
                "The Martian", "Good Omens", "The Midnight Library", "The Road",
                "Wild", "Into the Wild", "Shantaram", "Eat Pray Love",
                "Station Eleven", "The Glass Castle"
            },

            ["The Mind Hacker"] = new List<string>
            {
                "Thinking, Fast and Slow", "Atomic Habits", "Deep Work",
                "The Subtle Art...", "The War of Art"
            },

            ["The Feminine Voice"] = new List<string>
            {
                "Becoming"
            },

            ["Strategic Minds"] = new List<string>
            {
                "Sapiens", "The Art of War"
            },

            ["Health Reboot"] = new List<string>
            {
                "Why We Sleep", "Can't Hurt Me", "The Body Keeps the Score", "1984"
            },

            ["Philosopher's Path"] = new List<string>
            {
                "Man's Search for Meaning", "The Power of Now", "Meditations", "Ikigai"
            },

            ["The Innovators"] = new List<string>
            {
                "Steve Jobs", "The 4-Hour Workweek", "The Lean Startup"
            },

            ["Stories that Heal"] = new List<string>
            {
                "To Kill a Mockingbird", "Pride and Prejudice", "Educated",
                "The Catcher in the Rye", "The Diary of a Young Girl", "Becoming",
                "The Fault in Our Stars", "The Book Thief", "A Man Called Ove",
                "Normal People"
            },

            ["The Great Classics"] = new List<string>
            {
                "1984", "To Kill a Mockingbird", "The Great Gatsby",
                "The Catcher in the Rye", "Pride and Prejudice", "Jane Eyre",
                "Crime and Punishment", "Frankenstein", "Les Misérables",
                "The Brothers Karamazov", "The Hobbit", "Dune", "The Alchemist",
                "The Midnight Library", "The Artist's Way", "Meditations",
                "Sapiens", "Educated", "The Road", "Becoming", "Quiet"
            }
        };
    }

    public static List<ThemedCollection> GetThemedCollections()
    {
        return new List<ThemedCollection>
        {
            new ThemedCollection
            {
                Name = "Dopamine Detox Guide",
                Interests = new[] { "Habits", "Self-Help", "Health & Fitness" },
                Purposes = new[] { "Healthy Lifestyle", "Personal Growth & Self-Improvement" },
                Books = new[]
                {
                    "Atomic Habits", "Deep Work", "Digital Minimalism", "The Power of Now",
                    "Why We Sleep", "Can't Hurt Me", "Ikigai", "Essentialism"
                }
            },
            new ThemedCollection
            {
                Name = "Top 10 to Know History Inside Out",
                Interests = new[] { "History", "Politics", "Non-Fiction" },
                Purposes = new[] { "Academic & Educational Purposes", "Professional Development" },
                Books = new[]
                {
                    "Sapiens", "Guns, Germs, and Steel", "Why Nations Fail", "The Prince",
                    "A People's History of the United States", "A Brief History of Time",
                    "The Art of War", "The Diary of a Young Girl", "Man's Search for Meaning",
                    "The Righteous Mind"
                }
            },
            new ThemedCollection
            {
                Name = "The Mental Fitness Stack",
                Interests = new[] { "Psychology", "Habits", "Productivity" },
                Purposes = new[] { "Personal Growth & Self-Improvement", "Professional Development" },
                Books = new[]
                {
                    "Thinking, Fast and Slow", "Atomic Habits", "Deep Work", "Can't Hurt Me",
                    "Mindset", "The Subtle Art...", "Make Time", "The Power of Now", "Drive",
                    "The 7 Habits of Highly Effective People"
                }
            },
            new ThemedCollection
            {
                Name = "Creative Mind Unlocked",
                Interests = new[] { "Art", "Creativity", "Philosophy" },
                Purposes = new[] { "Creativity & Imagination" },
                Books = new[]
                {
                    "Steal Like an Artist", "The War of Art", "Big Magic", "The Artist's Way",
                    "Flow", "The Power of Now", "Letters to a Young Poet", "Ways of Seeing",
                    "On Writing", "Show Your Work"
                }
            },
            new ThemedCollection
            {
                Name = "Imaginative Realms & Escapes",
                Interests = new[] { "Fantasy", "Sci-Fi", "Fiction" },
                Purposes = new[] { "Entertainment and Relaxation", "Creativity & Imagination" },
                Books = new[]
                {
                    "Dune", "The Hobbit", "The Name of the Wind", "Ender's Game",
                    "Brave New World", "Fahrenheit 451", "The Left Hand of Darkness",
                    "The Martian", "Good Omens"
                }
            },
            new ThemedCollection
            {
                Name = "Stories That Make You Empathize",
                Interests = new[] { "Fiction", "Romance", "Memoir" },
                Purposes = new[] { "Social & Connection" },
                Books = new[]
                {
                    "To Kill a Mockingbird", "Pride and Prejudice", "Educated",
                    "The Catcher in the Rye", "The Diary of a Young Girl", "Becoming",
                    "The Fault in Our Stars", "The Book Thief", "A Man Called Ove",
                    "Normal People"
                }
            },
            new ThemedCollection
            {
                Name = "Life Lessons from Real Lives",
                Interests = new[] { "Memoir", "Biography", "Self-Help" },
                Purposes = new[] { "Inspiration & Motivation" },
                Books = new[]
                {
                    "Man's Search for Meaning", "Becoming", "Educated", "Can't Hurt Me",
                    "Steve Jobs", "Shoe Dog", "The Glass Castle", "Born a Crime",
                    "The Long Walk to Freedom", "Eat Pray Love"
                }
            },
            new ThemedCollection
            {
                Name = "The Builder's Toolkit",
                Interests = new[] { "Business", "Productivity", "Economics" },
                Purposes = new[] { "Professional Development" },
                Books = new[]
                {
                    "The 4-Hour Workweek", "Deep Work", "Thinking, Fast and Slow", "Drive",
                    "Essentialism", "Atomic Habits", "Outliers", "Grit", "Rework",
                    "Start With Why"
                }
            },
            new ThemedCollection
            {
                Name = "Scientific Curiosity Pack",
                Interests = new[] { "Biology", "Science", "Philosophy" },
                Purposes = new[] { "Academic & Educational Purposes", "Personal Growth & Self-Improvement" },
                Books = new[]
                {
                    "Sapiens", "A Brief History of Time", "The Gene", "The Selfish Gene",
                    "The Emperor of All Maladies", "Guns, Germs, and Steel",
                    "Thinking, Fast and Slow", "The Immortal Life of Henrietta Lacks",
                    "Cosmos", "The Body"
                }
            },
            new ThemedCollection
            {
                Name = "Brains and Biases",
                Interests = new[] { "Psychology", "Philosophy", "Self-Help" },
                Purposes = new[] { "Academic & Educational Purposes", "Personal Growth & Self-Improvement" },
                Books = new[]
                {
                    "Thinking, Fast and Slow", "The Power of Habit", "The Paradox of Choice",
                    "Blink", "Drive", "Mindset", "Nudge", "Predictably Irrational",
                    "The Art of Thinking Clearly", "Influence"
                }
            },
            new ThemedCollection
            {
                Name = "Mystery & Page-turners",
                Interests = new[] { "Detective", "Mystery", "Fiction", "Thriller" },
                Purposes = new[] { "Entertainment and Relaxation", "Creativity & Imagination" },
                Books = new[]
                {
                    "Murder on the Orient Express", "Gone Girl", "The Girl with the Dragon Tattoo",
                    "Sherlock Holmes", "The Da Vinci Code", "The Silent Patient",
                    "Big Little Lies", "Before I Go to Sleep", "In the Woods",
                    "And Then There Were None"
                }
            },
            new ThemedCollection
            {
                Name = "Classics You Shouldn't Miss",
                Interests = new[] { "Fiction", "Literature", "History" },
                Purposes = new[] { "Educational", "Inspiration & Motivation" },
                Books = new[]
                {
                    "1984", "To Kill a Mockingbird", "The Great Gatsby",
                    "The Catcher in the Rye", "Pride and Prejudice", "Jane Eyre",
                    "Crime and Punishment", "Frankenstein", "Les Misérables",
                    "The Brothers Karamazov"
                }
            },
            new ThemedCollection
            {
                Name = "Epic Journeys & Adventure",
                Interests = new[] { "Adventure", "Travel", "Memoir" },
                Purposes = new[] { "Entertainment and Relaxation", "Creativity & Imagination" },
                Books = new[]
                {
                    "Wild", "Into the Wild", "Shantaram", "Eat Pray Love", "The Road",
                    "Station Eleven", "The Glass Castle", "Becoming"
                }
            }
        };
    }
}