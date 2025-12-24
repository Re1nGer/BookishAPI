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
                u.StreakLengthInDays,
                u.PagesReadGoalInYear
            })
            .FirstAsync();

        var userTz = TZConvert.GetTimeZoneInfo(user.TimeZoneId ?? "UTC");
        var utcNow = DateTime.UtcNow;
        var goalMinutes = user.TimeLengthInMinutes ?? 0;
        var goalPages = user.PagesReadGoalInYear ?? 0;

        var currentStreak = await CalculateCurrentStreakAsync(userId, userTz, goalMinutes, goalPages, utcNow);
        var (minutesToday, pagesToday, isGoalMetToday) = await GetTodayProgressAsync(userId, userTz, goalMinutes, goalPages, utcNow);
        
        var notesCount = await _db.Notes
            .Where(j => j.CreatedAt >= utcNow.AddDays(-90))
            .Where(j => j.Book.UserId == userId)
            .CountAsync();

        return new StreakStatus
        {
            CurrentStreak = currentStreak,
            TargetDays = (int)user.StreakLengthInDays,
            MinutesToday = minutesToday,
            GoalMinutes = goalMinutes,
            Progress = goalMinutes > 0 ? (double)minutesToday / goalMinutes : 0,
            IsGoalMetToday = isGoalMetToday,
            IsAtRisk = currentStreak > 0 && !isGoalMetToday,
            PagesReadToday = pagesToday,
            NotesCount = notesCount
        };
    }

    private async Task<int> CalculateCurrentStreakAsync(
        Guid userId, TimeZoneInfo userTz, int goalMinutes, int goalPages, DateTime utcNow)
    {
        var cutoffDate = utcNow.AddDays(-90);

        var sessions = await _db.ReadingSessions
            .Where(s => s.Book.UserId == userId && s.EndTime >= cutoffDate)
            .Select(s => new { s.EndTime, s.DurationInSeconds, s.TimeZoneId })
            .ToListAsync();

        var readStats = await _db.ReadStats
            .Where(r => r.UserId == userId && r.ReadAt >= cutoffDate)
            .Select(r => new { r.ReadAt, r.PageNumber })
            .ToListAsync();

        var minutesByDay = sessions
            .GroupBy(s =>
            {
                var tz = TZConvert.GetTimeZoneInfo(s.TimeZoneId ?? userTz.Id);
                return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(s.EndTime, tz));
            })
            .ToDictionary(g => g.Key, g => g.Sum(s => s.DurationInSeconds / 60));

        var pagesByDay = readStats
            .GroupBy(r => DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(r.ReadAt, userTz)))
            .ToDictionary(g => g.Key, g => g.Sum(r => r.PageNumber));

        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(utcNow, userTz));
        var checkDate = today.AddDays(-1);
        var streak = 0;

        while (IsGoalMetOnDay(checkDate, minutesByDay, pagesByDay, goalMinutes, goalPages))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        if (IsGoalMetOnDay(today, minutesByDay, pagesByDay, goalMinutes, goalPages))
        {
            streak++;
        }

        return streak;
    }

    private bool IsGoalMetOnDay(
        DateOnly day,
        Dictionary<DateOnly, int> minutesByDay,
        Dictionary<DateOnly, int> pagesByDay,
        int goalMinutes,
        int goalPages)
    {
        var hadSession = minutesByDay.TryGetValue(day, out var minutes);
        var readThatDay = pagesByDay.TryGetValue(day, out var pages);

        var minutesGoalMet = hadSession && goalMinutes > 0 && minutes > goalMinutes;
        var pagesGoalMet = readThatDay && goalPages > 0 && pages > goalPages;

        return minutesGoalMet || pagesGoalMet;
    }

    private async Task<(int minutes, int pages, bool goalMet)> GetTodayProgressAsync(
        Guid userId, TimeZoneInfo userTz, int goalMinutes, int goalPages, DateTime utcNow)
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
            .Where(r => r.UserId == userId &&
                        r.ReadAt >= todayStart &&
                        r.ReadAt < todayEnd)
            .SumAsync(r => r.PageNumber);

        var minutesGoalMet = goalMinutes > 0 && minutes >= goalMinutes;
        var pagesGoalMet = goalPages > 0 && pages >= goalPages;

        return (minutes, pages, minutesGoalMet || pagesGoalMet);
    }
}
