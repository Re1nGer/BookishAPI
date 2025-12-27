using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace BookishAPI;


public class FirebaseService
{
    private readonly FirebaseMessaging _messaging;
    private readonly ILogger<FirebaseService> _logger;

    public FirebaseService(IConfiguration configuration, ILogger<FirebaseService> logger)
    {
        _logger = logger;

        // Initialize Firebase Admin SDK
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(configuration["Firebase:ServiceAccountKeyPath"])
            });
        }

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task<List<FirebaseResult>> SendToMultipleTokensAsync(List<string> tokens, FirebaseNotificationPayload payload)
    {
        var results = new List<FirebaseResult>();

        // Firebase supports batch sending to up to 500 tokens
        var batches = tokens.Chunk(500);

        foreach (var batch in batches)
        {
            var message = CreateMulticastMessage(batch.ToList(), payload);
            
            try
            {
                var response = await _messaging.SendEachForMulticastAsync(message);
                
                // Process individual results
                for (int i = 0; i < response.Responses.Count; i++)
                {
                    var token = batch.ElementAt(i);
                    var sendResponse = response.Responses[i];
                    
                    results.Add(new FirebaseResult
                    {
                        Token = token,
                        Success = sendResponse.IsSuccess,
                        MessageId = sendResponse.MessageId,
                        ErrorCode = sendResponse.Exception?.MessagingErrorCode?.ToString(),
                        ErrorMessage = sendResponse.Exception?.Message
                    });

                    // Log failed tokens for cleanup
                    if (!sendResponse.IsSuccess)
                    {
                        _logger.LogWarning($"Failed to send to token {token}: {sendResponse.Exception?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending batch notifications");
                
                // Mark all tokens in this batch as failed
                foreach (var token in batch)
                {
                    results.Add(new FirebaseResult
                    {
                        Token = token,
                        Success = false,
                        ErrorMessage = ex.Message
                    });
                }
            }
        }

        return results;
    }

    public async Task<FirebaseResult> SendToSingleTokenAsync(string token, FirebaseNotificationPayload payload)
    {
        var message = CreateMessage(token, payload);
        
        try
        {
            var messageId = await _messaging.SendAsync(message);
            return new FirebaseResult
            {
                Token = token,
                Success = true,
                MessageId = messageId
            };
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogError(ex, $"Firebase error sending to token {token}");
            return new FirebaseResult
            {
                Token = token,
                Success = false,
                ErrorCode = ex.MessagingErrorCode.ToString(),
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"General error sending to token {token}");
            return new FirebaseResult
            {
                Token = token,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private MulticastMessage CreateMulticastMessage(List<string> tokens, FirebaseNotificationPayload payload)
    {
        return new MulticastMessage()
        {
            Tokens = tokens,
            Notification = new Notification()
            {
                Title = payload.Title,
                Body = payload.Body,
                ImageUrl = payload.ImageUrl
            },
            Data = payload.Data,
            Android = new AndroidConfig()
            {
                Notification = new AndroidNotification()
                {
                    ClickAction = "FLUTTER_NOTIFICATION_CLICK", // For Flutter apps
                    Priority = NotificationPriority.HIGH
                }
            }
        };
    }

    private Message CreateMessage(string token, FirebaseNotificationPayload payload)
    {
        return new Message()
        {
            Token = token,
            Notification = new Notification()
            {
                Title = payload.Title,
                Body = payload.Body,
                ImageUrl = payload.ImageUrl
            },
            Data = payload.Data,
            /*
            Android = new AndroidConfig()
            {
                Notification = new AndroidNotification()
                {
                    ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                    Priority = NotificationPriority.HIGH
                }
            },
            Apns = new ApnsConfig()
            {
                Aps = new Aps()
                {
                    Alert = new ApsAlert()
                    {
                        Title = payload.Title,
                        Body = payload.Body
                    },
                    Badge = 1,
                    Sound = "default"
                }
            }
        */
        };
    }
}

public class FirebaseNotificationPayload
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Dictionary<string, string> Data { get; set; } = new();
}

public class FirebaseResult
{
    public string Token { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}
