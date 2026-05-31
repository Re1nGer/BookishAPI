using System.Text.Json;

namespace BookishAPI;

public class GoogleBooksClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";
    private readonly ILogger<GoogleBooksClient> _logger;
    private readonly CategoryMapper _categoryMapper;
    private readonly IConfiguration _configuration;

    public GoogleBooksClient(ILogger<GoogleBooksClient> logger, CategoryMapper categoryMapper, IConfiguration configuration)
    {
        _logger = logger;
        _categoryMapper = categoryMapper;
        _configuration = configuration;
        _httpClient = new HttpClient();
    }

    public async Task<GoogleBooksListDto> SearchBooksByTitleAsync(string title, int? maxResult = 10)
    {
        var apiKey = _configuration["Google:ApiKey"];
        try
        {
            var query = Uri.EscapeDataString(title);
            var response = await _httpClient.GetStringAsync($"{BaseUrl}?q={query}&maxResults={maxResult}&key={apiKey}");
            var result = JsonSerializer.Deserialize<GoogleBooksListDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            foreach (var book in result.Items)
            {
                var mappedCategories = _categoryMapper.MapCategories(book.VolumeInfo.Categories);
                
                book.VolumeInfo.Categories = mappedCategories
                    .Where(x => !string.IsNullOrEmpty(x.NormalizedCategory))
                    .Select(x => x.NormalizedCategory)
                    .Distinct()
                    .ToList();
            }
        

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error sending request");
            throw;
        }
    }
    public async Task<GoogleBooksItemDto> GetBookByVolumeId(string id)
    {
        var apiKey = _configuration["Google:ApiKey"];
        try
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/{id}?key={apiKey}");
            
            var result = JsonSerializer.Deserialize<GoogleBooksItemDto>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var mappedCategories = _categoryMapper.MapCategories(result.VolumeInfo.Categories);

            result.VolumeInfo.Description = StripHtmlTags(result.VolumeInfo.Description);
            
            result.VolumeInfo.Categories = mappedCategories
                .Where(x => !string.IsNullOrEmpty(x.NormalizedCategory))
                .Select(x => x.NormalizedCategory)
                .Distinct()
                .ToList();
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "error sending request");
            throw;
        }
    }
    private string StripHtmlTags(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        
        return System.Text.RegularExpressions.Regex.Replace(
            input,
            "<[^>]*(>|$)",
            string.Empty
        );
    }
}


public class GoogleBooksListDto
{
    public int TotalItems { get; set; }
    public List<GoogleBooksItemDto> Items { get; set; }
}

public class GoogleBooksItemDto
{
    public string Id { get; set; }
    public string Etag { get; set; }
    public string SelfLink { get; set; }
    public VolumeInfo VolumeInfo { get; set; }
}

public class VolumeInfo
{
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public string PublishedDate { get; set; }
    public string Description { get; set; }
    public int PageCount { get; set; }
    public string PrintType { get; set; }
    public List<string> Categories { get; set; }
    public double AverageRating { get; set; }
    public int RatingsCount { get; set; }
    public string MaturityRating { get; set; }
    public bool AllowAnonLogging { get; set; }
    public string ContentVersion { get; set; }
    public ImageLinks ImageLinks { get; set; }
    public string Language { get; set; }
    public string PreviewLink { get; set; }
    public string InfoLink { get; set; }
    public string CanonicalVolumeLink { get; set; }
}

public class ImageLinks
{
    public string SmallThumbnail { get; set; }
    public string Thumbnail { get; set; }
}