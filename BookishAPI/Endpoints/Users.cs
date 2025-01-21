using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
        
        group.MapGet("quotes", GetUserQuotes)
            .WithName("Get user's quotes")
            .WithSummary("Get all quotes of user");
        
        group.MapGet("note/type", GetNoteTypes)
            .WithName("Get user's note types")
            .WithSummary("Get all note types of user");
        
        group.MapPost("note/type", CreateNoteType)
            .WithName("Create user's note types")
            .WithSummary("Create note types of user");
        
        group.MapGet("books/{id}/notes", GetBookNotes)
            .WithName("Get book's notes")
            .WithSummary("Get all notes of book");
        
        group.MapGet("books/{id}/quotes", GetBookQuotes)
            .WithName("Get book's quotes")
            .WithSummary("Get all quotes of book");
        
        group.MapGet("books/notes", GetAllBooksNotes)
            .WithName("Get all books' notes")
            .WithSummary("Get all notes of all books");
        
        group.MapGet("books/quotes", GetAllBooksQuotes)
            .WithName("Get all books' quotes")
            .WithSummary("Get all quotes of all books");
        
        group.MapGet("collections", GetUserBookCollections)
            .WithName("Get user's book collections")
            .WithSummary("Get user's book collections");
        
        group.MapPost("collections", CreateUserBookCollection)
            .WithName("Create user's book collections")
            .WithSummary("Create user's book collections");

        return group;
    }
    private static async Task<IResult> CreateQuoteCollections(ClaimsPrincipal claimsPrincipal, BookAppContext db, NoteCollectionCreateRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var noteCollection = new QuoteCollection
        {
            Name = request.Name,
            UserId = parsedUserId
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

        var noteCollection = new NoteCollection
        {
            Name = request.Name,
            UserId = parsedUserId
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
            .Select(item => new BookCollectionWithCountDto(item.Id, item.Name, item.Quotes.Count()))
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
            .Select(item => new BookCollectionWithCountDto(item.Id, item.Name, item.Notes.Count()))
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
                .Select(item => new BookNote(item.Id, book.Title,
                    item.Content,
                    item.Type.Name,
                    item.Type.Color,
                    item.Type.Icon, item.CreatedAt))
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
            .Select(item => new CollectionWithCountDto(item.Id, item.Name, item.Books.Count))
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

        var collection = new BookCollection
        {
            UserId = Guid.Parse(userId),
            Name = request.Name
        };

        db.BookCollections.Add(collection);
        await db.SaveChangesAsync();

        return Results.Created($"/collections/{collection.Id}", collection);
        
    }
}