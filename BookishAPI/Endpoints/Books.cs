using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BookishAPI.Endpoints;

public static class Books
{
    public static RouteGroupBuilder MapBooksEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/books")
            .WithTags("Books")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("{bookId}/note/{noteId}", GetBooksNote)
            .WithName("Get Book's note")
            .WithSummary("Get Book's note by book id and note id");
        
        group.MapGet("{bookId}/quote/{quoteId}", GetBookQuote)
            .WithName("Get Book's quote")
            .WithSummary("Get Book's quote by book id and quote id");

        group.MapDelete("{bookId}/note/{noteId}", DeleteBooksNote)
            .WithName("Delete Book's note")
            .WithSummary("Delete Book's note by book id and note id");
        
        group.MapPut("{bookId}/note", ModifyBooksNote)
            .WithName("Modify Book's note")
            .WithSummary("Modify Book's note by book id");
        
        group.MapPost("{bookId}/note", CreateBook)
            .WithName("Create Book's note")
            .WithSummary("Create Book's note with book's id");
        
        group.MapDelete("{bookId}/quote/{quoteId}", DeleteBooksQuote)
            .WithName("Delete Book's quote")
            .WithSummary("Delete Book's quote by book id and quote id");
        
        
        group.MapPut("{bookId}/quote", ModifyBooksQuote)
            .WithName("Modify Book's quote")
            .WithSummary("Modify Book's quote by book id and quote id");
        
        group.MapPost("{bookId}/quote", CreateBooksQuote)
            .WithName("Create Book's quote")
            .WithSummary("Create Book's note with book's id");
            
        group.MapGet("interest-areas", GetInterestAreas)
            .WithName("Get User Interest Areas")
            .WithSummary("Get User Interest Areas");
            
        group.MapGet("reading-purposes", GetReadingPurposes)
            .WithName("Get User Reading Purposes")
            .WithSummary("Get User Reading Purposes");
        
        group.MapGet("interested-books", GetInterestedBooks)
            .WithName("Get User Interested Books")
            .WithSummary("Get User Interested Books");

        return group;
    }
    
    private static async Task<IResult> GetInterestedBooks(
        BookAppContext db)
    {
        var readingPurposes = await db.SelectedBooks.ToListAsync();
        return Results.Ok(readingPurposes);
    }
    
    private static async Task<IResult> GetReadingPurposes(
        BookAppContext db)
    {
        var readingPurposes = await db.ReadingPurposes.ToListAsync();
        return Results.Ok(readingPurposes);
    }
    
    private static async Task<IResult> GetInterestAreas(
        BookAppContext db)
    {
        var interestAreas = await db.InterestAreas.ToListAsync();
        return Results.Ok(interestAreas);
    }
    
    private static async Task<IResult> GetBooksNote(ClaimsPrincipal claimsPrincipal, BookAppContext db, int bookId, int noteId)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var hasNote = await db.Notes
            .Include(item => item.Book)
            .AnyAsync(item => item.Id == noteId && item.Book.Id == bookId && item.Book.UserId == parsedUserId);
        
        if (!hasNote)
        {
            return Results.NotFound();
        }

        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId);

        var note = await db.Notes
            .Include(item => item.RelatedQuote)
            .Include(item => item.Type)
            .Include(item => item.NoteCollections)
            .Include(item => item.SpacedRepetitionGroups)
            .Where(item => item.BookId == bookId && item.Id == noteId)
            .FirstOrDefaultAsync();

        var result = new SingleNoteDto(
            note.Id,
            note.Content,
            book.Title,
            note.Type.Id,
            note.RelatedQuote != null
                ? new QuoteDto(note.RelatedQuote.Id, book.Title, note.RelatedQuote.Content)
                : null,
            note.NoteCollections
                .Select(j => new CollectionDto(j.Id, j.Name)).ToList(),
            note.SpacedRepetitionGroups.Select(j => new RepetitionGroupDto() { Id = j.Id, Name = j.Name})
                .ToList()
            );

        return Results.Ok(result);
        
    }

    //perhaps it has to be a soft delete
    private static async Task<IResult> DeleteBooksNote(ClaimsPrincipal claimsPrincipal, BookAppContext db, int bookId, int noteId)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);

        var hasNote = await db.Notes
            .Include(item => item.Book)
            .AnyAsync(item => item.Id == noteId && item.Book.Id == bookId && item.Book.UserId == parsedUserId);
        
        if (!hasNote)
        {
            return Results.NotFound();
        }

        var note = await db.Notes
            .FirstOrDefaultAsync(item => item.Id == noteId
                                         && item.BookId == bookId);

        db.Notes.Remove(note);

        await db.SaveChangesAsync();

        return Results.Ok(note);
    }
    
    private static async Task<IResult> ModifyBooksNote(ClaimsPrincipal claimsPrincipal, int bookId, NoteModifyRequest request, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();
        
        var noteType = await db.NoteTypes
            .FirstOrDefaultAsync(item => item.Id == request.TypeId && item.UserId == parsedUserId);

        if (noteType is null)
        {
            return Results.NotFound("Note Type is not found");
        }

        var note = await db.Notes
            .Include(item => item.SpacedRepetitionGroups)
            .Include(item => item.RelatedQuote)
            .Include(item => item.NoteCollections)
            .FirstOrDefaultAsync(item => item.Id == request.Id && item.BookId == bookId);

        if (note is null)
        {
            return Results.NotFound("Note not found");
        }

        note.Content = request.Content;
        note.Type = noteType;
        
        var repetitionGroups = await db.SpacedRepetitionGroups
            .Where(item => request.RepetitionGroupIds.Contains(item.Id) && item.UserId == parsedUserId)
            .ToListAsync();

        if (repetitionGroups.Count > 0)
        {
            note.SpacedRepetitionGroups.Clear();
            note.SpacedRepetitionGroups = repetitionGroups;
        }
        else
        {
            note.SpacedRepetitionGroups.Clear();
        }

        var quote = await db.Quotes
            .Include(item => item.Book)
            .FirstOrDefaultAsync(item =>
                item.Id == request.QuoteId
                && item.BookId == bookId
                && item.Book.UserId == parsedUserId);

        if (quote is not null)
        {
            note.RelatedQuote = quote;
        }

        var noteCollections = await db.NoteCollections
            .Where(item => request.CollectionIds.Contains(item.Id) && item.UserId == parsedUserId)
            .ToListAsync();

        if (noteCollections.Count > 0)
        {
            note.NoteCollections.Clear();
            note.NoteCollections = noteCollections;
        }
        else
        {
            note.NoteCollections.Clear();
        }

        await db.SaveChangesAsync();

        return Results.Ok();
        
    }
    
    private static async Task<IResult> CreateBook(ClaimsPrincipal claimsPrincipal, int bookId, NoteCreateRequest request, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();

        var note = new Note
        {
            BookId = bookId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        var noteType = await db.NoteTypes
            .FirstOrDefaultAsync(item => item.Id == request.TypeId && item.UserId == parsedUserId);

        if (noteType is null)
        {
            return Results.NotFound("Note Type is not found");
        }
        
        note.Type = noteType;

        var repetitionGroups = await db.SpacedRepetitionGroups
            .Where(item => request.RepetitionGroupIds.Contains(item.Id) && item.UserId == parsedUserId)
            .ToListAsync();

        if (repetitionGroups.Count > 0)
        {
            note.SpacedRepetitionGroups = repetitionGroups;
        }

        var quote = await db.Quotes
            .Include(item => item.Book)
            .FirstOrDefaultAsync(item =>
                item.Id == request.QuoteId
                && item.BookId == bookId
                && item.Book.UserId == parsedUserId);

        if (quote is not null)
        {
            note.RelatedQuote = quote;
        }

        var noteCollections = await db.NoteCollections
            .Where(item => request.CollectionIds.Contains(item.Id) && item.UserId == parsedUserId)
            .ToListAsync();

        if (noteCollections.Count > 0)
        {
            note.NoteCollections = noteCollections;
        }

        db.Notes.Add(note);
        
        await db.SaveChangesAsync();

        return Results.Created($"/notes/{note.Id}", note);
    }
    
    private static async Task<IResult> DeleteBooksQuote(ClaimsPrincipal claimsPrincipal, int bookId, int quoteId, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();
        
        var quote = await db.Quotes
            .Include(item => item.RelatedNotes)
            .Include(item => item.QuoteCollections)
            .FirstOrDefaultAsync(item => item.Id == quoteId && item.BookId == book.Id);

        if (quote is null)
        {
            return Results.NotFound();
        }

        db.Quotes.Remove(quote);

        await db.SaveChangesAsync();

        return Results.Ok();
        
    }
    
    private static async Task<IResult> ModifyBooksQuote (ClaimsPrincipal claimsPrincipal, int bookId, QuoteModifyRequest request, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();
        
        var quote = await db.Quotes
            .Include(item => item.RelatedNotes)
            .Include(item => item.QuoteCollections)
            .Include(item => item.SpacedRepetitionGroups)
            .FirstOrDefaultAsync(item => item.Id == request.Id && item.BookId == book.Id);

        if (quote is null)
        {
            return Results.NotFound();
        }

        quote.Content = request.Content;

        //do we let users add quotes from other books ?
        if (request.NoteIds is not null && request.NoteIds.Length > 0)
        {
            var relatedNotes = await db.Notes
                .Include(item => item.Book)
                .Where(item => request.NoteIds.Contains(item.Id))
                .ToListAsync();

            if (quote?.RelatedNotes is not null)
            {
                quote.RelatedNotes = relatedNotes;
            }
        }
        else
        {
            quote.RelatedNotes = new List<Note>();
        }

        if (request.CollectionIds is not null && request.CollectionIds.Length > 0)
        {
            var quoteCollections = await db.QuoteCollections
                .Where(item => item.UserId == parsedUserId && request.CollectionIds.Contains(item.Id))
                .ToListAsync();
            
            quote.QuoteCollections = quoteCollections;
        }
        else
        {
            quote.QuoteCollections = new List<QuoteCollection>();
        }

        if (request.RepetitionGroupIds is not null && request.RepetitionGroupIds.Length > 0)
        {
            var repetitionGroups = await db.SpacedRepetitionGroups
                .Where(item => request.RepetitionGroupIds.Contains(item.Id) && item.UserId == parsedUserId)
                .ToListAsync();

            if (repetitionGroups.Count > 0)
            {
                quote.SpacedRepetitionGroups.Clear();
                quote.SpacedRepetitionGroups = repetitionGroups;
            }
        }
        else
        {
            quote.SpacedRepetitionGroups.Clear();
        }

        db.Quotes.Update(quote);

        await db.SaveChangesAsync();

        return Results.Ok();
        
    }
    
    private static async Task<IResult> CreateBooksQuote(ClaimsPrincipal claimsPrincipal, BookAppContext db, int bookId, QuoteCreateRequest request)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();
        
        var quote = new Quote
        {
            CreatedAt = DateTime.UtcNow,
            BookId = bookId,
            Content = request.Content,
        };

        //do we let users add quotes from other books ?
        if (request.NoteIds is not null && request.NoteIds.Length > 0)
        {
            var relatedNotes = await db.Notes
                .Include(item => item.Book)
                .Where(item => request.NoteIds.Contains(item.Id))
                .ToListAsync();
            
            quote.RelatedNotes = relatedNotes;
        }

        if (request.CollectionIds is not null && request.CollectionIds.Length > 0)
        {
            var quoteCollections = await db.QuoteCollections
                .Where(item => item.UserId == parsedUserId && request.CollectionIds.Contains(item.Id))
                .ToListAsync();
            
            quote.QuoteCollections = quoteCollections;
        }
        
        var repetitionGroups = await db.SpacedRepetitionGroups
            .Where(item => request.RepetitionGroupIds.Contains(item.Id) && item.UserId == parsedUserId)
            .ToListAsync();

        if (repetitionGroups.Count > 0)
        {
            quote.SpacedRepetitionGroups = repetitionGroups;
        }

        db.Quotes.Add(quote);
        
        await db.SaveChangesAsync();

        return Results.Created($"/quotes/{quote.Id}", quote);
        
    }
    private static async Task<IResult> GetBookQuote(ClaimsPrincipal claimsPrincipal, int bookId, int quoteId, BookAppContext db)
    {
        var userId = claimsPrincipal.Claims
            .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;

        var parsedUserId = Guid.Parse(userId);
        
        var book = await db.Books
            .FirstOrDefaultAsync(item => item.Id == bookId && item.UserId == parsedUserId);
        
        if (book == null) return Results.NotFound();
        
        var quote = await db.Quotes
            .AsNoTracking()
            .Include(item => item.RelatedNotes)
            .ThenInclude(item => item.Type)
            .Include(item => item.QuoteCollections)
            .Include(item => item.SpacedRepetitionGroups)
            .FirstOrDefaultAsync(item => item.Id == quoteId && item.BookId == book.Id);

        var relatedNotes = quote?.RelatedNotes.Select(item =>
            new NoteDto(
                item.Id,
                item.Content,
                item.Type.Name,
                item.Type.Color,
                item.Type.Icon,
                item.CreatedAt))
            .ToList();

        var result = new QuoteWithNotesDto(quote.Id,
            book.Title,
            quote.Content,
            quote.QuoteCollections.Select(item => new
                CollectionDto(item.Id, item.Name)).ToList(),
            relatedNotes,
            quote.SpacedRepetitionGroups
                .Select(a => new RepetitionGroupDto() { Id = a.Id, Name = a.Name})
                .ToList()
            );

        return Results.Ok(result);
        
    }
}