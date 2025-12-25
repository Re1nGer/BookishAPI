using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using BookishAPI;
using BookishAPI.Endpoints;
using BookishAPI.Schedulers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<BookAppContext>(u =>
{
    u.UseNpgsql(builder.Configuration.GetConnectionString("BackendContext"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MailerSendService>();
builder.Services.AddScoped<CodeGenerator>();
builder.Services.AddScoped<GoogleBooksClient>();
builder.Services.AddScoped<CategoryMapper>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<FirebaseService>();
builder.Services.AddScoped<StripeService>();
builder.Services.AddScoped<StreakService>();
//builder.Services.AddHostedService<NotificationSchedulerService>();
//builder.Services.AddHostedService<DailyReminderJob>();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.MaxDepth = 128; // Increase the maximum depth if needed
    options.SerializerOptions.WriteIndented = true; // Optional: for pretty-printing
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bookish API",
        Version = "v1",
        Description = "API serving bookish application"
    });

    // Add Bearer token authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB") 
                           ?? "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IGridFSBucket>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase("FileStorage");
    var options = new GridFSBucketOptions
    {
        BucketName = "files",
        ChunkSizeBytes = 1048576 // 1MB
    };
    return new GridFSBucket(database, options);
});


builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapBooksEndpoints();
app.MapUsersEndpoints();
app.MapStripeEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookAppContext>();
    if (dbContext.Database.GetPendingMigrations().Any())
    {
        dbContext.Database.Migrate();
    }
}

await app.SeedDataAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


/*
app.MapGet("/test", async (MailerSendService service, CodeGenerator generator) =>
{
    return Results.Ok(await service.SendEmailAsync("bekjonibr@gmail.com",
        "verification email", $"here's your verification code {generator.Generate4DigitCode()}"));
});
*/

app.MapGet("/categories", async () =>
{
    return Results.Ok(Categories.categories.Select((item, id) => new { item, id }));
});

app.MapPost("/forgot-password", async (
    ForgotPasswordRequest request,
    BookAppContext db,
    MailerSendService service,
    CodeGenerator generator) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(item => item.Email == request.Email);

    if (user is null) return Results.NotFound("User is not found");

    var verificationCode = new VerificationCode
    {
        CreatedAt = DateTime.UtcNow,
        Code = generator.Generate4DigitCode(),
        UserId = user.Id,
        IsUsed = false
    };
    
    db.VerificationCodes.Add(verificationCode);

    await db.SaveChangesAsync();
    
    return Results.Ok(await service.SendEmailAsync(user.Email,
        "verification email", $"here's your verification code {verificationCode.Code}"));
});


app.MapPost("/login", async (LoginRequest request, BookAppContext db, TokenService tokenService) =>
{
    var user = await db.Users
        .FirstOrDefaultAsync(item => item.Email == request.Email);
    
    if (user is null)
    {
        return Results.NotFound(new { Error = new { Email = "The user doesn't exist" }  });
    }

    var tokens = tokenService.GenerateTokens(user.Id);

    var isCorrectPassword = PasswordHasher.VerifyPassword(user.Password, request.Password);

    if (!isCorrectPassword)
    {
        return Results.BadRequest(new { Error = "Wrong password. Please try again!" });
    }

    return Results.Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken, userId=user.Id });
});

app.MapPost("/refresh", async (RefreshTokenRequest request, TokenService tokenService) =>
{
    var rotatedTokens = tokenService.RotateTokens(request.RefreshToken);
    return Results.Ok(new { accessToken = rotatedTokens.AccessToken, refreshToken = rotatedTokens.RefreshToken });
}).AllowAnonymous();

app.MapPost("/code-verify", async (CodeVerify request, BookAppContext db) =>
{
    var verificationCode = await db.VerificationCodes
        .Include(item => item.User)
        .FirstOrDefaultAsync(item =>
            item.Code == request.Code
            && item.User.Email == request.Email
            && !item.IsUsed
            && DateTime.UtcNow - item.CreatedAt <= TimeSpan.FromSeconds(30));

    if (verificationCode is null)
    {
        return Results.NotFound();
    }

    verificationCode.IsUsed = true;
    
    await db.SaveChangesAsync();
        
    return Results.Ok();
});

app.MapPost("/reset-password", async (ResetPasswordRequest request, BookAppContext db) =>
{
    var verificationCode = await db.VerificationCodes
        .Include(item => item.User)
        .FirstOrDefaultAsync(item =>
            item.Code == request.VerificationCode
            && item.User.Email == request.Email
            && !item.IsUsed
            && DateTime.UtcNow - item.CreatedAt <= TimeSpan.FromSeconds(300));
    
    if (verificationCode is null)
    {
        return Results.BadRequest(new { Error = "Session expired" });
    }

    if (request.NewPasswordRepeated != request.NewPassword)
    {
        return Results.BadRequest(new { Error = "Confirmed passwords don't match up" });
    }

    var user = await db.Users.FirstOrDefaultAsync(item => item.Email == request.Email);

    user.Password = PasswordHasher.HashPassword(request.NewPassword);

    await db.SaveChangesAsync();
        
    return Results.Ok("Password updated successfully");
});

    
// User endpoints
app.MapPost("/users", async (BookAppContext db, TokenService tokenService, UserRegistrationRequest request) =>
{
    if (db.Users.Any(item => item.Email == request.Email))
    {
        return Results.BadRequest(new { Error = new { UserExists = "This email is already signed !" } });
    }
    
    var errors = new Dictionary<string, string>();

    if (!EmailValidator.IsValid(request.Email))
    {
        errors.Add("email", "Invalid email format. Please try again.");
    }

    if (!PasswordValidator.IsValid(request.Password))
    {
        var passwordError = PasswordValidator.Validate(request.Password);
        errors.Add("password", passwordError);
    }

    var defaultNoteTypes = new List<NoteType>
    {
        new NoteType()
        {
            Name = "Thought",
            Color = "#519999",
            Icon = "👽",
        },
        new NoteType()
        {
            Name = "Question",
            Color = "#03679A",
            Icon = "🔍",
        },
        new NoteType()
        {
            Name = "Summary",
            Color = "#EEB63C",
            Icon = "👻",
        },
        new NoteType()
        {
            Name = "Fact",
            Color = "#F8846A",
            Icon = "💡",
        }
    };

    if (errors.Count != 0)
    {
        return Results.BadRequest(errors);
    }
    
    var user = new User
    {
        Username = request.Username,
        Email = request.Email,
        Password = PasswordHasher.HashPassword(request.Password),
        NoteTypes = defaultNoteTypes
    };

    db.Users.Add(user);
    
    await db.SaveChangesAsync();

    var tokens = tokenService.GenerateTokens(user.Id);

    return Results.Ok(new { accessToken = tokens.AccessToken, refreshToken = tokens.RefreshToken, userId=user.Id });
});

app.MapGet("/search/{title}", async (string title, [FromQuery] int? maxResult, GoogleBooksClient client) =>
{
    return Results.Ok(await client.SearchBooksByTitleAsync(title, maxResult));
});

app.MapGet("/book/{id}", async (string id, GoogleBooksClient client) =>
{
    return Results.Ok(await client.GetBookByVolumeId(id));
});

app.MapPost("/book", async (ClaimsPrincipal claimsPrincipal, BookAppContext db, BookAddRequest request) =>
{

    var userId = claimsPrincipal.Claims
        .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
    //gotta handle image url

    var parsedUserId = Guid.Parse(userId);

    var genres = request.Categories.Select(item => new Genre
    {
        Name = item
    }).ToList();
    
    
    //if book 
    var book = new Book
    {
        Title = request.Title,
        Description = request.Description,
        Author = string.Join(",", request.Authors),
        CurrentPage = 1,
        Genres = genres,
        Status = (BookStatus)request.Status,
        ImageUrl = request.ImageUrl,
        TotalPages = request.TotalPages,
        UserId = Guid.Parse(userId),
        CreatedAt = DateTime.UtcNow
    };

    if (request.Cover is not null)
    {
        book.BackgroundColor = request.Cover.BackgroundColor;
        book.TitleColor = request.Cover.TitleColor;
    }

    switch (request.Status)
    {
        case (int)BookStatus.Reading:
            book.StartedAt = DateTime.UtcNow;
            break;
        case (int)BookStatus.Finished:
            book.FinishedAt = DateTime.UtcNow;
            break;
    }

    var bookCollections = await db.BookCollections
        .Where(item => request.CollectionIds.Contains(item.Id) && item.UserId == parsedUserId)
        .ToListAsync();

    if (bookCollections.Count > 0)
    {
        book.BookCollections = bookCollections;
    }

    await db.Books.AddAsync(book);

    await db.SaveChangesAsync();
    
    return Results.Ok(new { book.Id });
    
}).RequireAuthorization();

//not used yet
/*
app.MapPut("/users/settings", async (ClaimsPrincipal claimsPrincipal, BookAppContext db, UserSettingsUpdateRequest request) =>
{
    var userId = claimsPrincipal.Claims
        .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
    
    var user = await db.Users
        .Include(u => u.Settings)
        .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
    
    if (user is null)
        return Results.NotFound();

    //user.Settings.NotificationsEnabled = request.NotificationsEnabled;
    user.Settings.TimeFormat = request.TimeFormat;

    await db.SaveChangesAsync();
    
    return Results.NoContent();
});
*/

//not used
app.MapPost("/users/{id}/verify-email", async (Guid id, string token, BookAppContext db) =>
{
    //gotta grab userId from jwt token
    
    var user = await db.Users
        .FirstOrDefaultAsync(item => item.Id == id);
    
    if (user == null) return Results.NotFound();

    // TODO: Verify token
    await db.SaveChangesAsync();

    return Results.Ok();
});

// Goal endpoints
//not used yet
app.MapPost("/users/goals", async (ClaimsPrincipal claimsPrincipal, GoalCreateRequest request, BookAppContext db) =>
{
    var userId = claimsPrincipal.Claims
        .FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
        
    if (userId is null) return Results.NotFound();

    var goal = new Goal
    {
        UserId = Guid.Parse(userId),
        Type = request.Type,
        Period = request.Period,
        Target = request.Target
    };

    db.Goals.Add(goal);
    await db.SaveChangesAsync();

    return Results.Created($"/goals/{goal.Id}", goal);
});

app.MapPost("/collections/{collectionId}/books", async (int collectionId, BookAddRequest request, BookAppContext db) =>
{
    var collection = await db.BookCollections
        .FirstOrDefaultAsync(item => item.Id == collectionId);
    
    if (collection is null) return Results.NotFound();

    var book = new Book
    {
        Title = request.Title,
        Description = request.Description,
        TotalPages = request.TotalPages
    };

    collection.Books.Add(book);
    
    await db.SaveChangesAsync();

    return Results.Created($"/books/{book.Id}", book);
}).RequireAuthorization();

app.MapDelete("/collections/{collectionId}/books/{bookId}", async (int collectionId, int bookId, BookAppContext db) =>
{
    var collection = await db.BookCollections
        .Include(c => c.Books)
        .FirstOrDefaultAsync(c => c.Id == collectionId);
    
    if (collection == null) return Results.NotFound();

    var book = collection.Books.FirstOrDefault(b => b.Id == bookId);
    if (book == null) return Results.NotFound();

    collection.Books.Remove(book);
    await db.SaveChangesAsync();

    return Results.NoContent();
}).RequireAuthorization();

app.MapPost("/users/spaced-repetition-groups/{groupId}/quote",
    async (int groupId, SpacedRepetitionItemAddRequest request, BookAppContext db) =>
{
    var group = await db.SpacedRepetitionGroups
        .Include(item => item.Quotes)
        .FirstOrDefaultAsync(item => item.Id == groupId);
    
    if (group == null) return Results.NotFound();

    var quote = await db.Quotes
        .FirstOrDefaultAsync(item => item.Id == request.QuoteId);
    
    if (quote is null) return Results.NotFound("Quote");
    
    group.Quotes.Add(quote);

    await db.SaveChangesAsync();
    
    return Results.NoContent();
}).RequireAuthorization();

// Search endpoints
app.MapGet("/quotes/search", async (string query, BookAppContext db) =>
{
    var quotes = await db.Quotes
        .Where(q => q.Content.Contains(query))
        .ToListAsync();

    return Results.Ok(quotes);
});

app.Run();

// Request DTOs

//Extract out into separate folder
public record LoginRequest(string Email, string? Password);
public record RefreshTokenRequest(string RefreshToken);
public record UserRegistrationRequest(string Username, string Email, string Password);
public record UserSettingsUpdateRequest(bool NotificationsEnabled, TimeFormat TimeFormat);
public record GoalCreateRequest(GoalType Type, GoalPeriod Period, int Target);
public record CollectionCreateRequest(string Name);
public record NoteCollectionCreateRequest(string Name);
public record BookAddRequest(
        string Title,
        string? Description,
        int TotalPages,
        string[] Authors,
        string[]? Categories,
        int[] CollectionIds,
        string? ImageUrl,
        int Status,
        BookCover? Cover
);

public class BookCover
{
    public BookCover(string titleColor, string backgroundColor)
    {
        TitleColor = titleColor;
        BackgroundColor = backgroundColor;
    }

    public string TitleColor { get; set; }
    public string BackgroundColor { get; set; }
}
public record QuoteCreateRequest(string Content, int[]? CollectionIds, int[]? RepetitionGroupIds, int[]? NoteIds);
public record QuoteModifyRequest(int Id, string Content, int[]? CollectionIds, int[]? RepetitionGroupIds, int[]? NoteIds);
public record NoteCreateRequest(
    string Content,
    int TypeId,
    int? QuoteId,
    int[]? CollectionIds,
    int[]? RepetitionGroupIds
);
public record NoteModifyRequest(
    int Id,
    string Content,
    int TypeId,
    int? QuoteId,
    int[]? CollectionIds,
    int[]? RepetitionGroupIds
);
public record NoteTypeCreateRequest(string Color, string Name, string Icon);
public record SpacedRepetitionGroupCreateRequest(string Name, DateTime RemindAt);
public record SpacedRepetitionItemAddRequest(int? QuoteId, int? NoteId);
public record ForgotPasswordRequest(string Email);
public record CodeVerify(string Code, string Email);
public record ResetPasswordRequest(string NewPassword, string NewPasswordRepeated, string Email, string VerificationCode);

public record BookCurrentPageUpdateRequest(int Page);

public record BookModifyRequest(int Id, string Title, string Author, int Status, string Description, int PageCount, int [] CollectionIds, string[] Categories);

//public record BookFilters(int[] Statuses, string[] Authors, string[] Categories, int[] Collections);

//Dtos
public record BookDto(
    int Id,
    string Title,
    string Author,
    string Description,
    int PageCount,
    int CurrentPage,
    DateTime? StartedAt,
    DateTime? FinishedAt,
    List<CategoryDto> Categories,
    string ImageUrl,
    int Status,
    List<NoteDto> Notes,
    List<CollectionDto> Collections,
    List<QuoteDto> Quotes,
    BookCover? Cover,
    ReadingSessionDto? Session = null
);

public class ReadingSessionDto
{
    public int BookId { get; set; }
    public DateTime EndTime { get; set; }
    public int EndPage { get; set; }
    public int DurationInSeconds { get; set; }
    public int PagesRead { get; set; }
}

public record NoteBookDto(int Id, string Name);

public record NoteTypeDto(int Id, string Name, string BgColor, string Icon);

public record NoteDto(int Id, string Content, string TypeName, string Color, string Icon, DateTime CreatedAt);
public record SingleNoteDto(
    int Id,
    string Content,
    string BookName,
    int TypeId,
    QuoteDto? Quote,
    List<CollectionDto> Collections,
    List<RepetitionGroupDto>? RepetitionGroups);

public record RepetitionGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public record CategoryDto(int Id, string Name);

public record AuthorDto(int Id, string Name);

public record CollectionDto(int Id, string Name);

public record CollectionWithCountDto(int Id, string Name, int BooksCount, int IconId);
public record BookCollectionWithCountDto(int Id, string Name, int NotesCount, int IconId);
public record QuoteCollectionWithCountDto(int Id, string Name, int NotesCount, int IconId);
public record NoteWithCountDto(int Id, string ImageUrl, string BookName, string Author, int NotesCount, BookCover? Cover);
public record BookQuoteWithCountDto(int Id, string ImageUrl, string BookName, string Author, int QuotesCount, BookCover? Cover);
public record BookNote(int Id, string BookName, string Text, string NoteTypeName, string NoteTypeColor, string NoteTypeIcon, DateTime Date);

//TODO: Fill in properties
public record QuoteDto(int Id, string BookName, string Text);
public record QuoteDtoTime(int Id, string BookName, string Text, DateTime CreatedAt);

public record QuoteWithNotesDto(int Id, string BookName, string Text, List<CollectionDto> Collections, List<NoteDto> Notes, List<RepetitionGroupDto> RepetitionGroups);
public record QuoteDtoWithCount(int Id, int BookId, string BookName, string Text, int NoteCount);

// Error Objects

public record SignUpErrors(string UserExists, string Email, string Password);

public record CreateReadEventRequest
{
    public int BookId { get; init; }
    public IFormFile? Image { get; init; }
    public string? Memo { get; init; }
    public short Rating { get; init; }
}

public record CreateRepetitionGroup(
    string Name,
    List<int> QuoteIds,
    List<int> NoteIds,
    RepetitionMode Mode,
    RepeatAt Time);

public class UpdateBookGoalRequest
{
    public int Amount { get; set; }
}

public class UserGoalState
{
    public int? PagesGoal { get; set; }
    public int? TimeGoalInMinutes { get; set; }
    public int? BooksGoal { get; set; }
    public int? CurrentAmountBooksGoal { get; set; }
    public int? CurrentAmountMinutes { get; set; }
    public List<BookShortDto> BooksReadWithinCurrentYear { get; set; }
}

public class BookShortDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime FinishedAt { get; set; }
    public BookCover? Cover { get; set; }
}

public record UserEvent(long Id, int BookId, string BookName, short Rating, string ImageUrl, DateTime FinishedAt, string? ImageId, string? Memo);
public record UserStat(
    List<Stat> ReadStats,
    List<BookReadStat> BookReadStats,
    long QuotesSaved,
    long NotesSaved,
    object TopCategories,
    BookAuthor[] TopAuthors,
    List<SessionStat> ReadingSessionStat);

public record BookCategory(int Id, string Name);
public record BookAuthor(int Id, string Name);
public record SpaceGroup(int Id, string Name, int IconId, int CardCount, byte ColorId);
public record UserMemoryEvent(long Id, int BookId, string Author, string BookName, short Rating, string ImageUrl, string? StartedAt, string? FinishedAt, string? ImageId, string? Memo);

public class BookReadStat
{
    public string Date { get; set; }
    public long BooksRead { get; set; }
}

public class Stat
{
    public string Date { get; set; }
    public long PagesRead { get; set; }
}

public class SessionStat
{
    public string Date { get; set; }
    public long SecondsRead { get; set; }
}

public class DueNotification
{
    public Guid UserId { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public DateTime ScheduledTime { get; set; }
    public int ScheduleId { get; set; } // To mark as sent later
}

public class RegisterTokenRequest
{
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = "android"; // "ios", "android", "web"
}

public class BookScoreCollection
{
    public string Name { get; set; }
    public string[] Interests { get; set; }
    public string[] Purposes { get; set; }
    public int[] BookIds { get; set; }
}

public class UpdateUserPreferencesRequest
{
    public int? BookAmountGoalInYear { get; set; }
    public DailyReminderAt? DailyReminderAt { get; set; }
    public int? TimeLengthInMinutes { get; set; }
    public StreakLengthInDays? StreakLengthInDays { get; set; }
    public int? PagesRead { get; set; }
    public bool? IsNotificationsEnabled { get; set; }
    public List<int> InterestAreaIds { get; set; } = new();
    public List<int> ReadingPurposeIds { get; set; } = new();
    public List<int> SelectedBookIds { get; set; } = new();
}

public class UserGoalsDto
{
    public int? PagesReadGoal { get; set; }
    public int? TimeLengthInMinutes { get; set; }
}

public class DailyReminderAt
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public string TimeZoneId { get; set; }
    public string TimeFormat { get; set; }
}

public class RepeatAt
{
    public int Hour { get; set; }
    public int Minute { get; set; }
    public string TimeZoneId { get; set; }
    public string TimeFormat { get; set; }
}

public class CollectionData
{
    public int[] PurposeIds { get; set; }
    public BookInfo[] Books { get; set; }
}

// Public class for API response
public class BookInfo
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
}

public class ThemedCollection
{
    public string Name { get; set; }
    public string[] Interests { get; set; }
    public string[] Purposes { get; set; }
    public string[] Books { get; set; }
}

public class CreateReadingSessionRequest
{
    public int BookId { get; set; }
    public int EndPage { get; set; }
    public int SessionLengthInSeconds { get; set; }
    public string TimeZoneId { get; set; }
    public int PagesRead { get; set; }
}

public class GoogleSignInRequest
{
    public string IdToken { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}