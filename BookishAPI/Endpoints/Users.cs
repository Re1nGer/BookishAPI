using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace BookishAPI.Endpoints;

public static class Users
{
    public static RouteGroupBuilder MapUsersEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/users")
            .WithTags("Users")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("quote-collections", CreateQuoteCollections)
            .WithName("Create user's quote collections")
            .WithSummary("Create User's quote collections");
        
        group.MapGet("quote-collections", GetQuoteCollections)
            .WithName("Get user's quote collections")
            .WithSummary("Get User's quote collections");
        
        group.MapGet("note-collections", GetNoteCollections)
            .WithName("Get user's note collections")
            .WithSummary("Get User's note collections");
        
        group.MapPost("note-collections", CreateNoteCollections)
            .WithName("Create user's note collections")
            .WithSummary("Create User's note collections");
        
        group.MapGet("quote-collections/{id}/quotes", GetCollectionQuotes)
            .WithName("Get all quotes of quote collection")
            .WithSummary("Get all quotes of quote collection by id");
        
        group.MapGet("note-collections/{id}/notes", GetCollectionNotes)
            .WithName("Get all notes of note collection")
            .WithSummary("Get all notes of note collection by id");
        
        group.MapGet("notes", GetUserNotes)
            .WithName("Get user's notes")
            .WithSummary("Get all notes of user");
        
        group.MapGet("notes/books", GetUserNotesBooks)
            .WithName("Get all notes' books")
            .WithSummary("Get all notes' books");
        
        group.MapGet("quotes", GetUserQuotes)
            .WithName("Get user's quotes")
            .WithSummary("Get all quotes of user");
        
        group.MapGet("note/type", GetNoteTypes)
            .WithName("Get user's note types")
            .WithSummary("Get all note types of user");
        
        group.MapPost("note/type", CreateNoteType)
            .WithName("Create user's note types")
            .WithSummary("Create note types of user");
        
        group.MapGet("books", GetUserBooks)
            .WithName("Get user's books")
            .WithSummary("Get all books");
        
        group.MapGet("book/{id}", GetUserBookById)
            .WithName("Get user's book by id")
            .WithSummary("Get user's book by id");
        
        group.MapGet("books/{id}/notes", GetBookNotes)
            .WithName("Get book's notes")
            .WithSummary("Get all notes of book");
        
        group.MapGet("books/{id}/quotes", GetBookQuotes)
            .WithName("Get book's quotes")
            .WithSummary("Get all quotes of book");
        
        group.MapPut("books/{id}/currentPage", UpdateBookCurrentReadPage)
            .WithName("Update book's current read page")
            .WithSummary("Update book's current read page");
        
        group.MapPut("book/{id}/status", UpdateBookStatus)
            .WithName("Update book's status")
            .WithSummary("Update book's status");
        
        group.MapPut("book", UpdateBook)
            .WithName("Update book")
            .WithSummary("Update book details");
        
        group.MapGet("books/notes", GetAllBooksNotes)
            .WithName("Get all books' notes")
            .WithSummary("Get all notes of all books");
        
        group.MapGet("books/quotes", GetAllBooksQuotes)
            .WithName("Get all books' quotes")
            .WithSummary("Get all quotes of all books");
        
        group.MapGet("books/authors", GetBooksAuthors)
            .WithName("Get all books' authors")
            .WithSummary("Get all books' authors");
        
        group.MapGet("collections", GetUserBookCollections)
            .WithName("Get user's book collections")
            .WithSummary("Get user's book collections");
        
        group.MapPost("collections", CreateUserBookCollection)
            .WithName("Create user's book collections")
            .WithSummary("Create user's book collections");
            
        group.MapGet("home", GetUserHome)
            .WithName("Get User's home books")
            .WithSummary("Get User's home books");
        
        group.MapPost("/{id}/files", UploadFile)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data");

        group.MapPost("books/read-events", CreateReadEvent)
            .DisableAntiforgery()
            .Accepts<IFormFile>("multipart/form-data");

        group.MapGet("read-events", GetUserReadEvents)
            .WithName("Get User's events")
            .WithSummary("Get User's events");

        group.MapGet("read-event/{id}", GetUserReadEvent)
            .WithName("Get User's event")
            .WithSummary("Get User's event");

        group.MapGet("memo/image/{id}", GetUserFiles)
            .WithName("Get User's event image")
            .WithSummary("Get User's event image")
            .AllowAnonymous();
            
        group.MapGet("stats", GetUserStats)
            .WithName("Get User's stats")
            .WithSummary("Get User's stats");
            
        group.MapPost("repetition-group", CreateRepetitionGroup)
            .WithName("Create User's repetition group")
            .WithSummary("Create User's repetition group");
        
        group.MapGet("repetition-groups", GetRepetitionGroups)
            .WithName("Get User's repetition group")
            .WithSummary("Get User's repetition group");
            
        group.MapGet("repetition-group/{groupId}", GetRepetitionGroupCards)
            .WithName("Get User's repetition group cards")
            .WithSummary("Get User's repetition group cards");
        
        group.MapPost("push-token", RegisterToken)
            .WithName("Register User's push token")
            .WithSummary("Register User's push token");
            
        return group;
    }
    private static async Task<IResult> GetRepetitionGroupCards(ClaimsPrincipal claimsPrincipal, BookAppContext db, int groupId)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var group = await db.SpacedRepetitionGroups
            .Include(j => j.Notes)
            .Include(j => j.Quotes)
            .FirstOrDefaultAsync(j => j.Id == groupId && j.UserId == parsedUserId);

        if (group is null)
        {
            return Results.NotFound("Repetition Group is null");
        }

        var notes = db.Notes.Where(j => group.Notes.Contains(j));

        var quotes = db.Quotes.Where(j => group.Quotes.Contains(j));

        var notesDto = notes.Select(j => new QuoteDto(j.Id, j.Book.Title, j.Content));

        var quotesDto = quotes.Select(j => new QuoteDto(j.Id, j.Book.Title, j.Content));

        var notesList = await notesDto.ToListAsync();

        var quotesList = await quotesDto.ToListAsync();
        
        notesList.AddRange(quotesList);

        return Results.Ok(notesList);
    }
    private static async Task<IResult> RegisterToken(ClaimsPrincipal claimsPrincipal,
        [FromServices]NotificationService notificationService, RegisterTokenRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        await notificationService.RegisterPushTokenAsync(
            parsedUserId,
            request.Token,
            request.Platform.ToLower());


        return Results.Ok(new { message = "Token registered successfully" });
    }
    
    private static async Task<IResult> GetRepetitionGroups(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var groups = await db.SpacedRepetitionGroups
            .Where(x => x.UserId == parsedUserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x =>
                new SpaceGroup(
                    x.Id,
                    x.Name,
                    x.IconId,
                    x.Notes.Count + x.Quotes.Count,
                    x.ColorId))
            .ToListAsync();
        
        return Results.Ok(groups);
    }
    
    private static async Task<IResult> CreateRepetitionGroup(
        ClaimsPrincipal claimsPrincipal,
        BookAppContext db,
        NotificationService notificationService,
        CreateRepetitionGroup request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var random = new Random();
            
            var iconId = random.Next(1, 40);

            var colorId = random.Next(0, 7);

            var group = new SpacedRepetitionGroup()
            {
                UserId = parsedUserId,
                Name = request.Name,
                Notes = db.Notes.Where(j => request.NoteIds.Contains(j.Id)).ToList(),
                Quotes = db.Quotes.Where(j => request.QuoteIds.Contains(j.Id)).ToList(),
                IconId = iconId,
                CreatedAt = DateTime.UtcNow,
                ColorId = (byte)colorId
            };

            await db.SpacedRepetitionGroups.AddAsync(group);
            
            await db.SaveChangesAsync();
            
            await notificationService.SaveNotificationSchedulesAsync(parsedUserId, group.Id, request.ScheduledTimes);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Results.StatusCode(500);
        }
        
        return Results.Ok();
    }
    
    private static async Task<IResult> GetUserStats(
        ClaimsPrincipal claimsPrincipal,
        BookAppContext db,
        [FromQuery] DateTime? from,
        DateTime? to)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var readStats = await db.ReadStats
            .Where(j => j.UserId == parsedUserId)
            .Where(j => from == null || j.ReadAt.Date >= from.Value.Date)
            .Where(j => to == null || j.ReadAt.Date <= to.Value.Date)
            .GroupBy(j => j.ReadAt.Date)
            .Select(g => new Stat
            {
                Day = g.Key.DayOfWeek.ToString(),
                PagesRead = g.Sum(a => a.PageNumber) 
            })
            .ToListAsync();

        var booksRead = await db.Books
            .Where(j => j.UserId == parsedUserId)
            .Where(j => from == null || j.FinishedAt >= from.Value)
            .Where(j => to == null || j.FinishedAt <= to.Value)
            .Where(j => j.Status == BookStatus.Finished)
            .LongCountAsync();

        var notesSaved = await db.Notes
            .Where(j => from == null || j.CreatedAt >= from.Value.Date)
            .Where(j => to == null || j.CreatedAt <= to.Value.Date)
            .Where(j => j.Book.UserId == parsedUserId)
            .LongCountAsync();

        var quotesSaved = await db.Quotes
            .Where(j => from == null || j.CreatedAt >= from.Value.Date)
            .Where(j => to == null || j.CreatedAt <= to.Value.Date)
            .Where(j => j.Book.UserId == parsedUserId)
            .LongCountAsync();

        var topCategoryNames = await db.Books
            .Where(j => j.UserId == parsedUserId)
            .Where(j => from == null || j.UpdatedAt >= from.Value)
            .Where(j => to == null || j.UpdatedAt <= to.Value)
            .SelectMany(j => j.Genres)
            .GroupBy(g => g.Name)
            .OrderByDescending(g => g.Count())
            .Take(5) // top 5 categories
            .Select(g => g.Key)
            .ToListAsync();

        var topCategories = await db.Books
            .Where(j => j.UserId == parsedUserId)
            .Where(j => j.Genres.Any(g => topCategoryNames.Contains(g.Name)))
            .OrderByDescending(j => j.Author)
            .Take(2)
            .Select(j => new BookCategory(j.Id, string.Join(",", j.Genres.Select(a => a.Name))))
            .ToArrayAsync();

        var topAuthors = await db.Books
            .Where(j => j.UserId == parsedUserId)
            .Where(j => from == null || j.UpdatedAt >= from.Value)
            .Where(j => to == null || j.UpdatedAt <= to.Value)
            .GroupBy(j => j.Author)
            .OrderByDescending(g => g.Count())
            .Take(2)
            .Select(g => new BookAuthor(g.First().Id, g.Key))
            .ToArrayAsync();

        
        return Results.Ok(new UserStat(
            readStats,
            0,
            booksRead,
            quotesSaved,
            notesSaved,
            topCategories,
            topAuthors));
    }
    
    private static async Task<IResult> GetUserReadEvents(ClaimsPrincipal claimsPrincipal, BookAppContext db, [FromQuery] DateTime? day)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var userEventsOptimal = await db.ReadEvents
            .Where(j => j.UserId == parsedUserId)
            .Where(j => day == null || j.CreatedAt.Date == day.Value.Date)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new UserEvent(
                j.Id, 
                j.Book.Id, 
                j.Book.Title,
                j.Rating,
                j.Book.ImageUrl, 
                j.Book.FinishedAt.GetValueOrDefault(),
                j.PhotoId,
                j.Memo))
            .ToListAsync();
        
        return Results.Ok(userEventsOptimal);
        
    }
    
    private static async Task<IResult> GetUserReadEvent(ClaimsPrincipal claimsPrincipal, BookAppContext db, long id)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var userEventsOptimal = await db.ReadEvents
            .Where(j => j.UserId == parsedUserId && j.Id == id)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new UserMemoryEvent(
                j.Id, 
                j.Book.Id, 
                j.Book.Author,
                j.Book.Title,
                j.Rating,
                j.Book.ImageUrl, 
                j.Book.StartedAt.HasValue ? j.Book.StartedAt.Value.Date.ToShortDateString() : null,
                j.Book.FinishedAt.HasValue ? j.Book.FinishedAt.Value.Date.ToShortDateString() : null,
                j.PhotoId,
                j.Memo))
            .FirstOrDefaultAsync();
        
        return Results.Ok(userEventsOptimal);
        
    }
    private static async Task<IResult> CreateQuoteCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db, NoteCollectionCreateRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var random = new Random();
        
        //40 is a max icon id
        //mapped on mobile client
        var iconId = random.Next(1, 40); 

        var noteCollection = new QuoteCollection
        {
            Name = request.Name,
            UserId = parsedUserId,
            IconId = iconId
        };

        await db.QuoteCollections.AddAsync(noteCollection);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
    private static async Task<IResult> CreateNoteCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db, NoteCollectionCreateRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var random = new Random();
        
        //40 is a max icon id
        //mapped on mobile client
        var iconId = random.Next(1, 40); 

        var noteCollection = new NoteCollection
        {
            Name = request.Name,
            UserId = parsedUserId,
            IconId = iconId
        };

        await db.NoteCollections.AddAsync(noteCollection);

        await db.SaveChangesAsync();

        return Results.Ok();
    }
    private static async Task<IResult> GetCollectionQuotes(ClaimsPrincipal claimsPrincipal, int id, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var collections = db.QuoteCollections
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Quotes)
            .ThenInclude(item => item.Book)
            .Where(item => item.UserId == parsedUserId && item.Id == id)
            .OrderByDescending(item => item.Id)
            .SelectMany(book => book.Quotes
                .Select(item => new QuoteDtoWithCount(item.Id, item.Book.Id, item.Book.Title,
                    item.Content, item.RelatedNotes.Count))
                .ToList());

        return Results.Ok(collections);

    }
    private static async Task<IResult> GetCollectionNotes(ClaimsPrincipal claimsPrincipal, int id, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var collections = db.NoteCollections
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Notes)
            .ThenInclude(item => item.Type)
            .Include(item => item.Notes)
            .ThenInclude(item => item.Book)
            .Where(item => item.UserId == parsedUserId && item.Id == id)
            .OrderByDescending(item => item.Id)
            .SelectMany(book => book.Notes
                .Select(item => new BookNote(item.Id, item.Book.Title,
                    item.Content,
                    item.Type.Name,
                    item.Type.Color,
                    item.Type.Icon, item.CreatedAt))
                .ToList());

        return Results.Ok(collections);
    }
    private static async Task<IResult> GetQuoteCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var collections = await db.QuoteCollections
            .Where(item => item.UserId == parsedUserId)
            .OrderByDescending(item => item.Id)
            .Select(item => new QuoteCollectionWithCountDto(item.Id, item.Name, item.Quotes.Count, item.IconId))
            .ToListAsync();

        return Results.Ok(collections);
    }
    private static async Task<IResult> GetNoteCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
    
        var parsedUserId = Guid.Parse(userId);
    
        var collections = await db.NoteCollections
            .Where(item => item.UserId == parsedUserId)
            .OrderByDescending(item => item.Id)
            .Select(item => new BookCollectionWithCountDto(item.Id, item.Name, item.Notes.Count, item.IconId))
            .ToListAsync();
    
        return Results.Ok(collections);
        
    }
    private static async Task<IResult> GetUserNotes(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var notes = db.Books
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Notes)
            .ThenInclude(item => item.Type)
            .Where(item => item.UserId == parsedUserId)
            .SelectMany(book => book.Notes
                .Select(item => new BookNote(
                    item.Id,
                    book.Title,
                    item.Content,
                    item.Type.Name,
                    item.Type.Color,
                    item.Type.Icon,
                    item.CreatedAt))
                .ToList());

        return Results.Ok(notes);
        
    }
    private static async Task<IResult> GetNoteTypes(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var noteTypes = await db.NoteTypes
            .Where(item => item.UserId == parsedUserId)
            .Select(item => new NoteTypeDto(item.Id, item.Name, item.Color, item.Icon))
            .ToListAsync();

        return Results.Ok(noteTypes);
        
    }
    private static async Task<IResult> CreateNoteType(ClaimsPrincipal claimsPrincipal, NoteTypeCreateRequest request, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var noteType = new NoteType
        {
            Name = request.Name,
            Color = request.Color,
            Icon = request.Icon,
            UserId = parsedUserId
        };
        
        db.NoteTypes.Add(noteType);
        
        await db.SaveChangesAsync();

        return Results.Ok();
        
    }
    private static async Task<IResult> GetUserQuotes(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var quotes = db.Books
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Quotes)
            .Where(item => item.UserId == parsedUserId)
            .SelectMany(book => book.Quotes
                .Select(item => new QuoteDto(
                    item.Id,
                    book.Title,
                    item.Content))
                .ToList());

        return Results.Ok(quotes);
        
    }
    private static async Task<IResult> GetBookNotes(ClaimsPrincipal claimsPrincipal, BookAppContext db, int id)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var book = await db.Books
            .Include(item => item.Notes)
            .ThenInclude(item => item.Type)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == parsedUserId);

        if (book is null)
        {
            return Results.NotFound();
        }

        var notes = book?.Notes
            .Select(item => new BookNote(item.Id, book.Title,
                item.Content,
                item.Type.Name,
                item.Type.Color,
                item.Type.Icon, item.CreatedAt))
            .ToList();

        return Results.Ok(notes ?? default);
    }
    private static async Task<IResult> GetAllBooksNotes(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var collections = await db.Books
            .Where(item => item.UserId == parsedUserId)
            .OrderByDescending(item => item.Id)
            .Select(item => new NoteWithCountDto(item.Id, item.ImageUrl, item.Title, item.Author, item.Notes.Count))
            .ToListAsync();

        return Results.Ok(collections);
    }
    private static async Task<IResult> GetAllBooksQuotes(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var bookQuotes = await db.Books
            .Where(item => item.UserId == parsedUserId)
            .OrderByDescending(item => item.Id)
            .Select(item => new BookQuoteWithCountDto(item.Id,
                item.ImageUrl, item.Title, item.Author, item.Quotes.Count))
            .ToListAsync();

        return Results.Ok(bookQuotes);
        
    }
    private static async Task<IResult> GetBookQuotes(ClaimsPrincipal claimsPrincipal, BookAppContext db, int id)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var book = await db.Books
            .Include(item => item.Quotes)
            .ThenInclude(item => item.RelatedNotes)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == parsedUserId);

        if (book is null)
        {
            return Results.NotFound();
        }

        var quotes = book.Quotes
            .Select(item => new QuoteDtoWithCount(
                item.Id,
                book.Id,
                book.Title,
                item.Content,
                item.RelatedNotes.Count))
            .ToList();

        return Results.Ok(quotes);
    }
    private static async Task<IResult> GetUserBookCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var collections = await db.BookCollections
            .Where(item => item.UserId == parsedUserId)
            .OrderByDescending(item => item.Id)
            .Select(item => new CollectionWithCountDto(item.Id, item.Name, item.Books.Count, item.IconId))
            .ToListAsync();

        return Results.Ok(collections);
        
    }
    private static async Task<IResult> CreateUserBookCollection(ClaimsPrincipal claimsPrincipal, CollectionCreateRequest request, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var user = await db.Users.FirstOrDefaultAsync(item => item.Id == parsedUserId);
        
        if (user == null) return Results.NotFound();
        
        var random = new Random();
        
        //40 is a max icon id
        //mapped on mobile client
        var iconId = random.Next(1, 40); 

        var collection = new BookCollection
        {
            UserId = Guid.Parse(userId),
            Name = request.Name,
            IconId = iconId
        };

        db.BookCollections.Add(collection);
        await db.SaveChangesAsync();

        return Results.Created($"/collections/{collection.Id}", collection);
        
    }
    private static async Task<IResult> UpdateBookCurrentReadPage(
        ClaimsPrincipal claimsPrincipal,
        int id,
        BookCurrentPageUpdateRequest request,
        BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == parsedUserId);

        if (book is null)
        {
            return Results.NotFound();
        }

        if (book.TotalPages < request.Page)
        {
            return Results.BadRequest(new { Error = "Page is greater than total pages in the book" });
        }

        var bookStat = new ReadStat
        {
            ReadAt = DateTime.UtcNow,
            PageNumber = request.Page - book.CurrentPage,
            BookId = id,
            UserId = parsedUserId
        };

        await db.ReadStats.AddAsync(bookStat);

        book.CurrentPage = request.Page;
        
        await db.SaveChangesAsync();
        
        return Results.Ok();
    }
    private static async Task<IResult> UpdateBook(ClaimsPrincipal claimsPrincipal, BookAppContext db, BookModifyRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .Include(item => item.BookCollections)
            .Include(item => item.Genres)
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.UserId == parsedUserId);

        if (book == null)
            return Results.NotFound("Book not found");

        book.Title = request.Title;
        book.Description = request.Description;
        book.TotalPages = request.PageCount;
        book.Author = request.Author;
        book.Status = (BookStatus)request.Status;

        var genres = request.Categories.Select(item => new Genre
        {
            Name = item
        }).ToList();
        
        book.Genres.Clear();
        book.Genres = genres;
        
        var bookCollections = await db.BookCollections
                                      .Where(c => request.CollectionIds.Contains(c.Id) 
                                                  && c.UserId == parsedUserId)
                                      .ToListAsync();
        
        book.BookCollections.Clear();
        book.BookCollections = bookCollections;

        await db.SaveChangesAsync();

        return Results.Ok();
    }
    private static async Task<IResult> GetUserNotesBooks(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var books = db.Books
            .AsNoTracking()
            .Where(item => item.UserId == parsedUserId);

        books = books
            .OrderBy(item => item.StartedAt);

        return Results.Ok(await books.Select(item => new NoteBookDto(item.Id, item.Title)).ToListAsync());
    }
    private static async Task<IResult> GetBooksAuthors(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var books = await db.Books
            .AsNoTracking()
            .Where(item => item.UserId == parsedUserId)
            .Select(item => item.Author).ToListAsync();

        var authorDtos = books
            .SelectMany(authorString => authorString?.Split(',') ?? Array.Empty<string>())
            .Select(author => author.Trim())
            .Where(author => !string.IsNullOrWhiteSpace(author))
            .Distinct()
            .Select((authorName, index) => new AuthorDto(index, authorName))
            .ToList();

        return Results.Ok(authorDtos);
    }
    private static async Task<IResult> UpdateBookStatus(ClaimsPrincipal claimsPrincipal, int id, int statusId, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == parsedUserId);

        if (book is null)
        {
            return Results.NotFound();
        }

        book.Status = (BookStatus)statusId;

        switch (book.Status)
        {
            case BookStatus.Finished:
                book.FinishedAt = DateTime.UtcNow;
                break;
            case BookStatus.ToRead:
                book.FinishedAt = null;
                book.StartedAt = null;
                break;
            case BookStatus.Reading:
                book.FinishedAt = null;
                book.StartedAt = DateTime.UtcNow;
                break;
        }

        book.UpdatedAt = DateTime.UtcNow;
        //db.Books.Update(book);

        await db.SaveChangesAsync();
        
        return Results.Ok();
    }
    private static async Task<IResult> GetUserBooks(ClaimsPrincipal claimsPrincipal, int[]? statuses, string[]? authors, string[]? categories, int[]? collections, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var books = db.Books
            .Include(item => item.Genres)
            .Include(item => item.BookCollections)
            .AsNoTracking()
            .Where(item => item.UserId == parsedUserId);

        if (statuses?.Length > 0)
        {
            books = books.Where(item => statuses.Contains((int)item.Status));
        }
        
        if (authors?.Length > 0)
        {
            books = books.Where(item => authors.Contains(item.Author)); //Could be some problems here
        }
        
        if (categories?.Length > 0)
        {
            var normalizedCategories = categories.Select(c => c.ToUpper()).ToArray();
            
            books = books.Where(item => item.Genres
                    .Any(k => normalizedCategories.Contains(k.Name.ToUpper())));
        }
        
        if (collections?.Length > 0)
        {
            books = books.Where(item => item.BookCollections
                .Any(k => collections.Contains(k.Id)));
        }

        books = books
            .OrderByDescending(item => item.UpdatedAt);

        return Results.Ok(await books.ToListAsync());

    }
    private static async Task<IResult> GetUserBookById(ClaimsPrincipal claimsPrincipal, int id, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
        var parsedUserId = Guid.Parse(userId);

        var book = await db.Books
            .Include(item => item.Notes)
            .ThenInclude(item => item.Type)
            .Include(item => item.BookCollections)
            .Include(item => item.Quotes)
            .Include(item => item.Genres)
            .AsSplitQuery()
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == parsedUserId);

        if (book is null)
        {
            return Results.NotFound("Book not found");
        }


        var bookDto = new BookDto(
            Id: book.Id,
            Title: book.Title,
            Description: book.Description,
            PageCount: book.TotalPages,
            CurrentPage: book.CurrentPage,
            StartedAt: book.StartedAt,
            FinishedAt: book.FinishedAt,
            Author: book.Author,
            Status: (int)book.Status,
            ImageUrl: book.ImageUrl,
            Categories: book.Genres.Select(item =>  //Extract into separate map method
                new CategoryDto(item.Id, item.Name)).ToList(),
            Notes: book.Notes
                .Select(item => 
                    new NoteDto(Id: item.Id,
                    Content: item.Content,
                    TypeName: item.Type.Name,
                    Color: item.Type.Color,
                    Icon: item.Type.Icon,
                    CreatedAt: item.CreatedAt)
                ).OrderByDescending(item => item.CreatedAt)
                .ToList(),
            Collections: book.BookCollections.Select(item =>
                new CollectionDto(item.Id, item.Name)).ToList(),
            Quotes: book.Quotes.Select(item =>
                new QuoteDto(item.Id, book.Title, item.Content))
                .ToList());

        return Results.Ok(bookDto);
    }
    private static async Task<IResult> GetUserHome(ClaimsPrincipal claimsPrincipal, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var user = await db.Users.FirstOrDefaultAsync(item => item.Id == parsedUserId);
        
        if (user == null) return Results.NotFound("The user is not found");

        var book = await db.Books
            .Where(j => j.Status == BookStatus.Reading)
            .Include(item => item.Notes)
            .ThenInclude(item => item.Type)
            .Include(item => item.BookCollections)
            .Include(item => item.Quotes)
            .Include(item => item.Genres)
            .OrderByDescending(j => j.UpdatedAt)
            .AsSplitQuery()
            .FirstOrDefaultAsync();

        if (book is null) return Results.NotFound("The book is not found");

        var bookDto = new BookDto(
            Id: book.Id,
            Title: book.Title,
            Description: book.Description,
            PageCount: book.TotalPages,
            CurrentPage: book.CurrentPage,
            StartedAt: book.StartedAt,
            FinishedAt: book.FinishedAt,
            Author: book.Author,
            Status: (int)book.Status,
            ImageUrl: book.ImageUrl,
            Categories: book.Genres.Select(item =>  //Extract into separate map method
                new CategoryDto(item.Id, item.Name)).ToList(),
            Notes: book.Notes
                .Select(item => 
                    new NoteDto(Id: item.Id,
                    Content: item.Content,
                    TypeName: item.Type.Name,
                    Color: item.Type.Color,
                    Icon: item.Type.Icon,
                    CreatedAt: item.CreatedAt)
                ).OrderByDescending(item => item.CreatedAt)
                .ToList(),
            Collections: book.BookCollections.Select(item =>
                new CollectionDto(item.Id, item.Name))
                .ToList(),
            Quotes: book.Quotes.Select(item =>
                    new QuoteDto(item.Id, book.Title, item.Content))
                .ToList());

        return Results.Ok(bookDto);
    }

    private static async Task<IResult> GetUserFiles(ClaimsPrincipal claimsPrincipal, string id, IGridFSBucket gridFS)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return Results.BadRequest("Invalid file ID format");

            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            
            var fileInfo = await gridFS.Find(filter).FirstOrDefaultAsync();
        
            if (fileInfo == null)
                return Results.NotFound("File not found");

            // Extract content type and original filename from metadata
            var contentType = "application/octet-stream"; // Default
            
            if (fileInfo.Metadata != null && fileInfo.Metadata.Contains("contentType"))
                contentType = fileInfo.Metadata["contentType"].AsString;

            // Download file as a stream
            var stream = await gridFS.OpenDownloadStreamAsync(objectId);
        
            return Results.File(stream, contentType, fileInfo.Filename);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error downloading file: {ex.Message}");
        }
    }
    private static async Task<IResult> UploadFile(int userId, [FromForm] FileUploadRequest request, IGridFSBucket gridFS)
    {
        try
        {
            if (!request.Files.Any())
                return Results.BadRequest("No files uploaded");

            var uploadResults = new List<object>();

            foreach (var file in request.Files)
            {
                var metadata = new BsonDocument
                {
                    { "fileName", file.FileName },
                    { "contentType", file.ContentType },
                    { "uploadDate", DateTime.UtcNow },
                    { "userId", userId }
                };

                // Open file stream for reading
                using var stream = file.OpenReadStream();
            
                // Upload to GridFS
                var id = await gridFS.UploadFromStreamAsync(
                    file.FileName,
                    stream,
                    new GridFSUploadOptions { Metadata = metadata }
                );

                uploadResults.Add(new { FileId = id.ToString(), file.FileName });
            }

            return Results.Ok(new { Files = uploadResults });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error uploading files: {ex.Message}");
        }
    }
    
    private static async Task<IResult> GetUserFile(ClaimsPrincipal claimsPrincipal, string id, IGridFSBucket gridFS)
    {
        try
        {
            var userId = claimsPrincipal.Claims
                .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

            var parsedUserId = Guid.Parse(userId);
        
            if (!ObjectId.TryParse(id, out var objectId))
                return Results.BadRequest("Invalid file ID format");

            // Find file info first to get metadata
            var filter = Builders<GridFSFileInfo>.Filter.And(
                Builders<GridFSFileInfo>.Filter.Eq("_id", objectId),
                Builders<GridFSFileInfo>.Filter.Eq("metadata.userId", parsedUserId)
            );
        
            var fileInfo = await gridFS.Find(filter).FirstOrDefaultAsync();
        
            if (fileInfo == null)
                return Results.NotFound("File not found or doesn't belong to the specified user");

            var contentType = "application/octet-stream"; // Default
            if (fileInfo.Metadata != null && fileInfo.Metadata.Contains("contentType"))
                contentType = fileInfo.Metadata["contentType"].AsString;

            var stream = await gridFS.OpenDownloadStreamAsync(objectId);
        
            return Results.File(stream, contentType, fileInfo.Filename);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error downloading file: {ex.Message}");
        }
    }
    
    private static async Task<IResult> CreateReadEvent(
        ClaimsPrincipal claimsPrincipal,
        BookAppContext db,
        IGridFSBucket gridFS,
        [FromForm]CreateReadEventRequest request)
    {
        
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var user = await db.Users.FirstOrDefaultAsync(item => item.Id == parsedUserId);
        
        if (user == null) return Results.NotFound("The user is not found");
        
        ObjectId id = default;

        if (request.Image is not null)
        {
            var secureFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
            
            var metadata = new BsonDocument
            {
                { "fileName",  secureFileName},
                { "contentType", request.Image.ContentType },
                { "uploadDate", DateTime.UtcNow },
                { "userId", userId }
            };
            
            // Open file stream for reading
            using var stream = request.Image.OpenReadStream();
        
            // Upload to GridFS
            id = await gridFS.UploadFromStreamAsync(
                request.Image.FileName,
                stream,
                new GridFSUploadOptions { Metadata = metadata }
            );
        }


        var readEvent = new ReadEvent()
        {
            CreatedAt = DateTime.UtcNow,
            Rating = request.Rating,
            Memo = request.Memo,
            PhotoId = request.Image is not null ? id.ToString() : null,
            BookId = request.BookId,
            UserId = parsedUserId
        };

        await db.ReadEvents.AddAsync(readEvent);

        await db.SaveChangesAsync();

        return Results.Ok(new { readEvent.Id });
    }
}

public class FileUploadRequest
{
    public List<IFormFile> Files { get; set; } = new();
}
