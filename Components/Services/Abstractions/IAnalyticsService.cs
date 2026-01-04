using MYMAUIAPP.Components.Model.Enums;

namespace MYMAUIAPP.Components.Services.Abstractions;

public interface IAnalyticsService
{
    Task<AnalyticsSnapshot> GetSnapshotAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default);
}

public sealed record AnalyticsSnapshot(
    int TotalEntries,
    int CurrentStreak,
    int LongestStreak,
    List<DateOnly> MissedDays,
    MoodDistribution Distribution,
    string? MostFrequentMood,
    List<TagUsage> TopTags,
    List<WordCountPoint> WordCountTrend
);

public sealed record MoodDistribution(double PositivePct, double NeutralPct, double NegativePct);

public sealed record TagUsage(string TagName, int Count);

public sealed record WordCountPoint(DateOnly Date, int WordCount);
