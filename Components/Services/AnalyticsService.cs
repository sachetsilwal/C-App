using MYMAUIAPP.Components.Model.Enums;
using MYMAUIAPP.Components.Repositories.Abstractions;
using MYMAUIAPP.Components.Services.Abstractions;

namespace MYMAUIAPP.Components.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IJournalEntryRepository _entries;

    public AnalyticsService(IJournalEntryRepository entries) => _entries = entries;

    public async Task<AnalyticsSnapshot> GetSnapshotAsync(DateOnly? from, DateOnly? to, CancellationToken ct = default)
    {
        var list = await _entries.SearchAsync(
            query: null,
            from: from,
            to: to,
            moodIds: null,
            tagIds: null,
            ct: ct);

        var total = list.Count;

        // Mood distribution based on PRIMARY mood (required)
        int pos = 0, neu = 0, neg = 0;
        var moodFreq = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var e in list)
        {
            var primary = e.EntryMoods.FirstOrDefault(x => x.IsPrimary)?.Mood;
            if (primary is null) continue;

            moodFreq[primary.Name] = moodFreq.TryGetValue(primary.Name, out var c) ? c + 1 : 1;

            switch (primary.MoodGroup)
            {
                case MoodGroup.Positive: pos++; break;
                case MoodGroup.Neutral: neu++; break;
                case MoodGroup.Negative: neg++; break;
            }
        }

        var dist = total == 0
            ? new MoodDistribution(0, 0, 0)
            : new MoodDistribution(
                PositivePct: Math.Round(pos * 100.0 / total, 2),
                NeutralPct: Math.Round(neu * 100.0 / total, 2),
                NegativePct: Math.Round(neg * 100.0 / total, 2)
            );

        var mostFrequentMood = moodFreq.Count == 0
            ? null
            : moodFreq.OrderByDescending(kv => kv.Value).First().Key;

        // Top tags (count per tag across entries)
        var tagCount = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var e in list)
        {
            foreach (var t in e.EntryTags.Select(x => x.Tag?.Name).Where(n => !string.IsNullOrWhiteSpace(n))!)
                tagCount[t!] = tagCount.TryGetValue(t!, out var c) ? c + 1 : 1;
        }

        var topTags = tagCount
            .OrderByDescending(kv => kv.Value)
            .Take(12)
            .Select(kv => new TagUsage(kv.Key, kv.Value))
            .ToList();

        // Word count trend (per day)
        var trend = list
            .OrderBy(x => x.EntryDate)
            .Select(x => new WordCountPoint(x.EntryDate, x.WordCount))
            .ToList();

        // Streaks / missed days across full history (not filtered by date range)
        // Streak computation should use all entries so it’s meaningful.
        var all = await _entries.SearchAsync(null, null, null, null, null, ct);
        var (currentStreak, longestStreak, missedDays) = ComputeStreaks(all);

        return new AnalyticsSnapshot(
            TotalEntries: total,
            CurrentStreak: currentStreak,
            LongestStreak: longestStreak,
            MissedDays: missedDays,
            Distribution: dist,
            MostFrequentMood: mostFrequentMood,
            TopTags: topTags,
            WordCountTrend: trend
        );
    }

    private static (int current, int longest, List<DateOnly> missed) ComputeStreaks(List<MYMAUIAPP.Components.Model.JournalEntry> entries)
    {
        if (entries.Count == 0)
            return (0, 0, new List<DateOnly>());

        var dates = entries
            .Select(e => e.EntryDate)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var set = new HashSet<DateOnly>(dates);

        var min = dates.First();
        var today = DateOnly.FromDateTime(DateTime.Now);

        // Missed days from min..today (inclusive) excluding entries
        var missed = new List<DateOnly>();
        for (var d = min; d <= today; d = d.AddDays(1))
        {
            if (!set.Contains(d))
                missed.Add(d);
        }

        // Longest streak
        int longest = 1, run = 1;
        for (int i = 1; i < dates.Count; i++)
        {
            if (dates[i] == dates[i - 1].AddDays(1))
            {
                run++;
                longest = Math.Max(longest, run);
            }
            else run = 1;
        }

        // Current streak: consecutive days ending today (if today exists) else ending last entry day
        int current = 0;
        var cursor = today;

        // If no entry today, current streak is 0 (strict interpretation)
        if (!set.Contains(today))
            return (0, longest, missed);

        while (set.Contains(cursor))
        {
            current++;
            cursor = cursor.AddDays(-1);
        }

        return (current, longest, missed);
    }
}
