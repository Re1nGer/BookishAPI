using Microsoft.EntityFrameworkCore;

namespace BookishAPI;

public class NotificationService
{
    private readonly BookAppContext _context;
    private readonly ILogger<BookAppContext> _logger;

    public NotificationService(BookAppContext context, ILogger<BookAppContext> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<DueNotification>> GetDueNotificationsAsync()
    {
        var now = DateTime.UtcNow;

        // Super simple query - just get unsent notifications that are due
        var dueNotifications = await _context.UserGroupNotificationSchedules
            .Where(schedule => !schedule.IsSent &&
                               schedule.ScheduledTime <= now)
            .Include(schedule => schedule.Group)
            .Select(schedule => new DueNotification
            {
                UserId = schedule.UserId,
                GroupId = schedule.GroupId,
                GroupName = schedule.Group.Name,
                ScheduledTime = schedule.ScheduledTime,
                ScheduleId = schedule.Id
            })
            .ToListAsync();

        _logger.LogInformation($"Found {dueNotifications.Count} due notifications");
        return dueNotifications;
    }

    public async Task<List<UserPushToken>> GetUserActiveTokensAsync(Guid userId)
    {
        return await _context.UserPushTokens
            .Where(t => t.UserId == userId && t.IsActive)
            .ToListAsync();
    }

    public async Task LogNotificationAsync(Guid userId, int groupId, List<FirebaseResult> results)
    {
        // Just log what happened
        var logs = results.Select(result => new NotificationLog
        {
            UserId = userId,
            GroupId = groupId,
            Status = result.Success ? "sent" : "failed",
            FirebaseResponse = System.Text.Json.JsonSerializer.Serialize(result),
            SentAt = DateTime.UtcNow
        }).ToList();

        _context.NotificationLogs.AddRange(logs);

        // Deactivate invalid tokens
        var invalidTokens = results
            .Where(r => !r.Success && IsTokenInvalid(r.ErrorCode))
            .Select(r => r.Token)
            .ToList();

        if (invalidTokens.Any())
        {
            await DeactivateInvalidTokensAsync(invalidTokens);
            _logger.LogWarning($"Deactivated {invalidTokens.Count} invalid tokens");
        }

        await _context.SaveChangesAsync();
    }

    public async Task MarkNotificationAsSentAsync(int scheduleId)
    {
        var schedule = await _context.UserGroupNotificationSchedules
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule != null)
        {
            schedule.IsSent = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RegisterPushTokenAsync(Guid userId, string token, string platform)
    {
        var existingToken = await _context.UserPushTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.DeviceToken == token && t.Platform == platform);

        if (existingToken == null)
        {
            var newToken = new UserPushToken
            {
                UserId = userId,
                DeviceToken = token,
                Platform = platform,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserPushTokens.Add(newToken);
            _logger.LogInformation($"Registered new push token for User {userId}, Platform {platform}");
            await _context.SaveChangesAsync();
            
        }
    }

    public async Task SaveNotificationSchedulesAsync(Guid userId, int groupId, List<DateTime> scheduledTimes)
    {
        var newSchedules = scheduledTimes.Select(time => new UserGroupNotificationSchedule
        {
            UserId = userId,
            GroupId = groupId,
            ScheduledTime = time,
            IsSent = false,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _context.UserGroupNotificationSchedules.AddRange(newSchedules);
        
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Saved {newSchedules.Count} notification schedules for User {userId}, Group {groupId}");
    }

    public async Task DeactivateInvalidTokensAsync(List<string> invalidTokens)
    {
        if (!invalidTokens.Any()) return;

        var tokensToDeactivate = await _context.UserPushTokens
            .Where(t => invalidTokens.Contains(t.DeviceToken) && t.IsActive)
            .ToListAsync();

        foreach (var token in tokensToDeactivate)
        {
            token.IsActive = false;
            token.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Deactivated {tokensToDeactivate.Count} invalid tokens");
    }

    private static bool IsTokenInvalid(string? errorCode)
    {
        return errorCode switch
        {
            "UNREGISTERED" => true,
            "INVALID_REGISTRATION" => true,
            "REGISTRATION_TOKEN_NOT_REGISTERED" => true,
            "INVALID_ARGUMENT" => true,
            _ => false
        };
    }
}
