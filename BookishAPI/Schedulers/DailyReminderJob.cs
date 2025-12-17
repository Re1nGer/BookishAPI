using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;

namespace BookishAPI.Schedulers;

public class DailyReminderJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyReminderJob> _logger;

    public DailyReminderJob(
        IServiceScopeFactory scopeFactory,
        ILogger<DailyReminderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Streak Reminder Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing streak reminders");
            }

            // Check every minute
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessRemindersAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookAppContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
        var firebaseService = scope.ServiceProvider.GetRequiredService<FirebaseService>();

        var now = DateTime.UtcNow;

        // Get users with reminders enabled
        var users = await context.Users
            .Where(u => u.IsNotificationsEnabled && 
                        u.TimeZoneId != null)
            .Select(u => new
            {
                u.Id,
                u.TimeZoneId,
                u.DailyReminderAt,
                u.TimeLengthInMinutes,
                u.StreakLengthInDays
            })
            .ToListAsync(stoppingToken);

        var usersToNotify = new List<Guid>();

        foreach (var user in users)
        {
            if (ShouldSendReminder(user.TimeZoneId!, user.DailyReminderAt, now))
            {
                usersToNotify.Add(user.Id);
            }
        }

        if (!usersToNotify.Any())
        {
            return;
        }

        _logger.LogInformation("Found {Count} users to send streak reminders", usersToNotify.Count);

        // Get streak status for these users
        var streakStatuses = await GetStreakStatusesAsync(context, usersToNotify, now, stoppingToken);

        foreach (var userId in usersToNotify)
        {
            if (stoppingToken.IsCancellationRequested) break;

            try
            {
                var status = streakStatuses.GetValueOrDefault(userId);
                
                // Only send if they haven't met today's goal
                if (status == null || status.IsGoalMetToday)
                {
                    continue;
                }

                var tokens = await notificationService.GetUserActiveTokensAsync(userId);
                if (!tokens.Any())
                {
                    _logger.LogDebug("No active tokens for user {UserId}", userId);
                    continue;
                }

                var (title, body) = BuildNotificationMessage(status);

                var results = await firebaseService.SendToMultipleTokensAsync(
                    tokens.Select(t => t.DeviceToken).ToList(),
                    new FirebaseNotificationPayload()
                    {
                        Title = title,
                        Body = body,
                        Data = new Dictionary<string, string>
                        {
                            ["type"] = "streak_reminder",
                            ["currentStreak"] = status.CurrentStreak.ToString(),
                            ["minutesRemaining"] = (status.GoalMinutes - status.MinutesToday).ToString()
                        }
                    }
                );

                // Log results and handle invalid tokens
                await notificationService.LogNotificationAsync(userId, groupId: 0, results);

                _logger.LogInformation(
                    "Sent streak reminder to user {UserId}, streak: {Streak}", 
                    userId, status.CurrentStreak);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send reminder to user {UserId}", userId);
            }
        }
    }

    private bool ShouldSendReminder(string timeZoneId, TimeOnly reminderTime, DateTime utcNow)
    {
        try
        {
            var tz = TZConvert.GetTimeZoneInfo(timeZoneId);
            var userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);

            return userLocalTime.Hour == reminderTime.Hour &&
                   userLocalTime.Minute == reminderTime.Minute;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
    }

    private async Task<Dictionary<Guid, StreakStatus>> GetStreakStatusesAsync(
        BookAppContext context,
        List<Guid> userIds,
        DateTime utcNow,
        CancellationToken stoppingToken)
    {
        var result = new Dictionary<Guid, StreakStatus>();

        // Get users with their reading sessions from today
        var usersWithData = await context.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.TimeZoneId,
                u.TimeLengthInMinutes,
                u.StreakLengthInDays,
                u.ReadStats,
                u.PagesReadGoalInYear,
                // Get recent sessions (last 48 hours to be safe across timezones)
                RecentSessions = u.Books
                    .SelectMany(b => b.ReadingSessions)
                    .Where(s => s.EndTime >= utcNow.AddHours(-48))
                    .Select(s => new { s.EndTime, s.DurationInSeconds, s.TimeZoneId })
                    .ToList()
            })
            .ToListAsync(stoppingToken);

        foreach (var user in usersWithData)
        {
            try
            {
                var tz = TZConvert.GetTimeZoneInfo(user.TimeZoneId ?? "UTC");
                var userLocalNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);
                var today = DateOnly.FromDateTime(userLocalNow);

                // Calculate today's reading
                var minutesToday = user.RecentSessions
                    .Where(s =>
                    {
                        var sessionTz = TZConvert.GetTimeZoneInfo(s.TimeZoneId);
                        var sessionLocalTime = TimeZoneInfo.ConvertTimeFromUtc(s.EndTime, sessionTz);
                        return DateOnly.FromDateTime(sessionLocalTime) == today;
                    })
                    .Sum(s => s.DurationInSeconds / 60);

                var pagesReadToday = user.ReadStats
                    .Where(item => item.ReadAt.Date == DateTime.UtcNow.Date)
                    .Sum(a => a.PageNumber);

                var goalMinutes = (int)user.TimeLengthInMinutes;

                // For streak calculation, we'd need historical data
                // This is simplified - ideally you'd have a UserStreak table
                var currentStreak = await CalculateCurrentStreakAsync(
                    context, user.Id, tz, goalMinutes, utcNow, stoppingToken);

                result[user.Id] = new StreakStatus
                {
                    CurrentStreak = currentStreak,
                    TargetDays = (int)user.StreakLengthInDays,
                    MinutesToday = minutesToday,
                    GoalMinutes = goalMinutes,
                    Progress = (double)minutesToday / goalMinutes,
                    IsGoalMetToday = minutesToday >= goalMinutes || pagesReadToday >= user.PagesReadGoalInYear,
                    IsAtRisk = currentStreak > 0 && minutesToday < goalMinutes
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate streak for user {UserId}", user.Id);
            }
        }

        return result;
    }

    private async Task<int> CalculateCurrentStreakAsync(
        BookAppContext context,
        Guid userId,
        TimeZoneInfo userTz,
        int goalMinutes,
        DateTime utcNow,
        CancellationToken stoppingToken)
    {
        // Get all sessions for the last 90 days (max streak we care about)
        var sessions = await context.ReadingSessions
            .Where(s => s.Book.UserId == userId && s.EndTime >= utcNow.AddDays(-90))
            .Select(s => new { s.EndTime, s.DurationInSeconds, s.TimeZoneId })
            .ToListAsync(stoppingToken);

        // Group by day in user's timezone
        var minutesByDay = sessions
            .GroupBy(s =>
            {
                var sessionTz = TZConvert.GetTimeZoneInfo(s.TimeZoneId ?? userTz.Id);
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(s.EndTime, sessionTz);
                return DateOnly.FromDateTime(localTime);
            })
            .ToDictionary(
                g => g.Key,
                g => g.Sum(s => s.DurationInSeconds / 60)
            );

        var userLocalNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, userTz);
        var today = DateOnly.FromDateTime(userLocalNow);
        var streak = 0;

        // Start from yesterday and count backwards
        var checkDate = today.AddDays(-1);

        while (true)
        {
            if (minutesByDay.TryGetValue(checkDate, out var minutes) && minutes >= goalMinutes)
            {
                streak++;
                checkDate = checkDate.AddDays(-1);
            }
            else
            {
                break;
            }
        }

        // If they met today's goal, add 1
        if (minutesByDay.TryGetValue(today, out var todayMinutes) && todayMinutes >= goalMinutes)
        {
            streak++;
        }

        return streak;
    }

    private (string Title, string Body) BuildNotificationMessage(StreakStatus status)
    {
        var minutesRemaining = status.GoalMinutes - status.MinutesToday;

        if (status.CurrentStreak == 0)
        {
            return (
                "Start your reading streak today! 📚",
                $"Read for {minutesRemaining} minutes to begin your journey."
            );
        }

        if (status.CurrentStreak >= 7)
        {
            return (
                $"🔥 {status.CurrentStreak} day streak at risk!",
                $"Just {minutesRemaining} minutes to keep your amazing streak alive!"
            );
        }

        return (
            "Don't break your streak! 📖",
            $"You're on a {status.CurrentStreak}-day streak. Read {minutesRemaining} more minutes today!"
        );
    }
}

public record StreakStatus
{
    public int CurrentStreak { get; init; }
    public int NotesCount { get; set; }
    public int TargetDays { get; init; }
    public int MinutesToday { get; init; }
    public int GoalMinutes { get; init; }
    public double Progress { get; init; }
    public bool IsGoalMetToday { get; init; }
    public bool IsAtRisk { get; init; }
    public int PagesReadToday { get; set; }
}
