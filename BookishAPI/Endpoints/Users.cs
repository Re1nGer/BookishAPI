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
    private static async Task<IResult> UpdateBookCurrentReadPage(ClaimsPrincipal claimsPrincipal, int id, BookCurrentPageUpdateRequest request, BookAppContext db)
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

        db.Books.Update(book);

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
            .OrderByDescending(item => item.StartedAt);

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
}