namespace BookishAPI.Schedulers;

public class NotificationSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationSchedulerService> _logger;
    private readonly TimeSpan _pollingInterval = TimeSpan.FromSeconds(45); // Check every minute

    public NotificationSchedulerService(
        IServiceProvider serviceProvider,
        ILogger<NotificationSchedulerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Scheduler Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingNotifications();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notifications");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        _logger.LogInformation("Notification Scheduler Service stopped");
    }

    private async Task ProcessPendingNotifications()
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepo = scope.ServiceProvider.GetRequiredService<NotificationService>();
        var firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();

        try
        {
            // Get all notifications due now
            var dueNotifications = await notificationRepo.GetDueNotificationsAsync();

            _logger.LogInformation($"Found {dueNotifications.Count} notifications to process");

            foreach (var notification in dueNotifications)
            {
                try
                {
                    await ProcessSingleNotification(notification, notificationRepo, firebaseService);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to process notification for User {notification.UserId}, Group {notification.GroupId}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting due notifications from database");
        }
    }

    private async Task ProcessSingleNotification(
        DueNotification notification,
        NotificationService notificationRepo,
        FirebaseService firebaseService)
    {
        // Get user's push tokens
        var tokens = await notificationRepo.GetUserActiveTokensAsync(notification.UserId);

        if (!tokens.Any())
        {
            _logger.LogWarning($"No active tokens found for User {notification.UserId}");
            // Still mark as sent to avoid retrying
            await notificationRepo.MarkNotificationAsSentAsync(notification.ScheduleId);
            return;
        }

        // Create notification payload
        var payload = new FirebaseNotificationPayload
        {
            Title = "Time to Review!",
            Body = $"Your spaced repetition group '{notification.GroupName}' is ready for review",
            Data = new Dictionary<string, string>
            {
                { "path", "/(auth)/revise" },
                { "groupId", notification.GroupId.ToString() },
                { "groupName", notification.GroupName },
                { "userId", notification.UserId.ToString() }
            }
        };

        // Send to all user devices
        var results = await firebaseService.SendToMultipleTokensAsync(tokens.Select(t => t.DeviceToken).ToList(), payload);

        // Log the notification
        //await notificationRepo.LogNotificationAsync(notification.UserId, notification.GroupId, results);

        // Mark this schedule entry as sent (regardless of success/failure to avoid duplicates)
        await notificationRepo.MarkNotificationAsSentAsync(notification.ScheduleId);

        _logger.LogInformation($"Processed notification for User {notification.UserId}, Group {notification.GroupId}. Scheduled for: {notification.ScheduledTime}");
    }
}