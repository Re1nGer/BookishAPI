using System.Text.Json;

namespace BookishAPI;

public class BookSearchResult
{
    public VolumeInfo volumeInfo { get; set; }
}

public class VolumeInfo
{
    public string title { get; set; }
    public string[] authors { get; set; }
    public string publisher { get; set; }
    public string publishedDate { get; set; }
    public string description { get; set; }
    public string isbn_13 { get; set; }
}

public class GoogleBooksClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.googleapis.com/books/v1/volumes";

    public GoogleBooksClient()
    {
        _httpClient = new HttpClient();
    }

    public async Task<JsonElement> SearchBooksByTitleAsync(string title, int? maxResult = 10)
    {
        var query = Uri.EscapeDataString(title);
        var response = await _httpClient.GetStringAsync($"{BaseUrl}?q={query}&maxResults={maxResult}");
        var result = JsonSerializer.Deserialize<JsonElement>(response);

        return result;
    }
}
