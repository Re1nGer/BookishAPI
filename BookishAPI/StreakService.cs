using BookishAPI.Schedulers;
using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;
namespace BookishAPI;

public class StreakService
{
    private readonly BookAppContext _db;

    public StreakService(BookAppContext db)
    {
        _db = db;
    }

    public async Task<StreakStatus> GetStreakStatusAsync(Guid userId)
    {
        var user = await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.TimeZoneId,
                u.TimeLengthInMinutes,
                u.StreakLengthInDays
            })
            .FirstAsync();

        var userTz = TZConvert.GetTimeZoneInfo(user.TimeZoneId ?? "UTC");
        var utcNow = DateTime.UtcNow;
        var goalMinutes = (int)user.TimeLengthInMinutes;

        var currentStreak = await CalculateCurrentStreakAsync(userId, userTz, goalMinutes, utcNow);
        var (minutesToday, pages, isGoalMetToday) = await GetTodayProgressAsync(userId, userTz, goalMinutes, utcNow);

        return new StreakStatus
        {
            CurrentStreak = currentStreak,
            TargetDays = (int)user.StreakLengthInDays,
            MinutesToday = minutesToday,
            GoalMinutes = goalMinutes,
            Progress = (double)minutesToday / goalMinutes,
            IsGoalMetToday = isGoalMetToday,
            IsAtRisk = currentStreak > 0 && !isGoalMetToday,
            PagesReadToday = pages
        };
    }

    private async Task<int> CalculateCurrentStreakAsync(
        Guid userId, TimeZoneInfo userTz, int goalMinutes, DateTime utcNow)
    {
        var sessions = await _db.ReadingSessions
            .Where(s => s.Book.UserId == userId && s.EndTime >= utcNow.AddDays(-90))
            .Select(s => new { s.EndTime, s.DurationInSeconds, s.TimeZoneId })
            .ToListAsync();

        var minutesByDay = sessions
            .GroupBy(s =>
            {
                var tz = TZConvert.GetTimeZoneInfo(s.TimeZoneId ?? userTz.Id);
                return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(s.EndTime, tz));
            })
            .ToDictionary(g => g.Key, g => g.Sum(s => s.DurationInSeconds / 60));

        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utcNow, userTz));
        var checkDate = today.AddDays(-1);
        var streak = 0;

        while (minutesByDay.TryGetValue(checkDate, out var mins) && mins >= goalMinutes)
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        if (minutesByDay.TryGetValue(today, out var todayMins) && todayMins >= goalMinutes)
        {
            streak++;
        }

        return streak;
    }

    private async Task<(int minutes, int pages, bool goalMet)> GetTodayProgressAsync(
        Guid userId, TimeZoneInfo userTz, int goalMinutes, DateTime utcNow)
    {
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utcNow, userTz));
        var todayStart = TimeZoneInfo.ConvertTimeToUtc(today.ToDateTime(TimeOnly.MinValue), userTz);
        var todayEnd = todayStart.AddDays(1);

        var minutes = await _db.ReadingSessions
            .Where(s => s.Book.UserId == userId && 
                        s.EndTime >= todayStart && 
                        s.EndTime < todayEnd)
            .SumAsync(s => s.DurationInSeconds / 60);
        
        var pages = await _db.ReadStats
            .Where(s => s.Book.UserId == userId && 
                        s.ReadAt >= todayStart && 
                        s.ReadAt < todayEnd)
            .SumAsync(s => s.PageNumber);

        return (minutes, pages, minutes >= goalMinutes);
    }
}