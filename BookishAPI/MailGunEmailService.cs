using System.Text;
using System.Text.Json;

namespace BookishAPI;

public class MailerSendService
{
    private const string Domain = "info@trial-351ndgw2poqgzqx8.mlsender.net"; // Your Mailgun domain
    private readonly IConfiguration _configuration;

    public MailerSendService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string textContent)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_configuration["MailerSend:ApiUrl"])
        };
        
        httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["MailerSend:ApiToken"]}");
        var emailData = new
        {
            from = new { email = Domain },
            to = new[] { new { email = toEmail } },
            subject,
            text = textContent,
        };

        var json = JsonSerializer.Serialize(emailData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("email", content);

        return response.IsSuccessStatusCode;
    }
}

