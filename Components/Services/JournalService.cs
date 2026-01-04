using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;
using MYMAUIAPP.Components.Services.Abstractions;

namespace MYMAUIAPP.Components.Services;

public class JournalService : IJournalService
{
    private readonly IJournalEntryRepository _entries;
    private readonly ITagRepository _tags;
    private readonly IMoodRepository _moods;

    public JournalService(IJournalEntryRepository entries, ITagRepository tags, IMoodRepository moods)
    {
        _entries = entries;
        _tags = tags;
        _moods = moods;
    }

    public Task<JournalEntry?> GetEntryAsync(DateOnly date, CancellationToken ct = default)
        => _entries.GetByDateAsync(date, ct);

    public async Task<JournalEntry> UpsertEntryAsync(
        DateOnly date,
        string title,
        string markdown,
        int? categoryId,
        int primaryMoodId,
        List<int> secondaryMoodIds,
        List<int> tagIds,
        List<string> customTags,
        CancellationToken ct = default)
    {
        title = (title ?? "").Trim();
        markdown = markdown ?? "";

        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.");
        if (string.IsNullOrWhiteSpace(markdown)) throw new ArgumentException("Content is required.");

        // Validate moods: primary required, secondary max 2, no duplicates
        secondaryMoodIds ??= new();
        secondaryMoodIds = secondaryMoodIds.Distinct().Where(id => id != primaryMoodId).ToList();
        if (secondaryMoodIds.Count > 2) throw new ArgumentException("You can select up to two secondary moods.");

        // Ensure primary mood exists
        var primaryMood = await _moods.GetByIdAsync(primaryMoodId, ct);
        if (primaryMood is null) throw new ArgumentException("Invalid primary mood.");

        foreach (var sid in secondaryMoodIds)
        {
            var m = await _moods.GetByIdAsync(sid, ct);
            if (m is null) throw new ArgumentException("Invalid secondary mood selection.");
        }

        // Ensure/normalize custom tags (create if missing)
        customTags ??= new();
        var normalizedCustomTags = customTags
            .Select(t => (t ?? "").Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var customTagEntities = new List<Tag>();
        foreach (var t in normalizedCustomTags)
            customTagEntities.Add(await _tags.CreateCustomAsync(t, ct));

        // Merge tagIds + customTagEntities ids
        tagIds ??= new();
        var allTagIds = tagIds
            .Concat(customTagEntities.Select(t => t.Id))
            .Distinct()
            .ToList();

        var entry = await _entries.GetByDateAsync(date, ct);

        if (entry is null)
        {
            entry = new JournalEntry
            {
                EntryDate = date,
                Title = title,
                ContentMarkdown = markdown,
                CategoryId = categoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                WordCount = CountWords(markdown)
            };

            entry.EntryMoods = BuildMoods(primaryMoodId, secondaryMoodIds);
            entry.EntryTags = allTagIds.Select(tid => new EntryTag { TagId = tid }).ToList();

            await _entries.AddAsync(entry, ct);
            await _entries.SaveChangesAsync(ct);
            return entry;
        }

        // Update existing entry for that day (CRUD rule)
        entry.Title = title;
        entry.ContentMarkdown = markdown;
        entry.CategoryId = categoryId;
        entry.UpdatedAt = DateTime.UtcNow;
        entry.WordCount = CountWords(markdown);

        // Replace moods/tags (simpler, consistent)
        entry.EntryMoods.Clear();
        foreach (var em in BuildMoods(primaryMoodId, secondaryMoodIds))
            entry.EntryMoods.Add(em);

        entry.EntryTags.Clear();
        foreach (var tid in allTagIds)
            entry.EntryTags.Add(new EntryTag { TagId = tid });

        await _entries.UpdateAsync(entry, ct);
        await _entries.SaveChangesAsync(ct);
        return entry;
    }

    public async Task DeleteEntryAsync(DateOnly date, CancellationToken ct = default)
    {
        var entry = await _entries.GetByDateAsync(date, ct);
        if (entry is null) return;

        await _entries.DeleteAsync(entry, ct);
        await _entries.SaveChangesAsync(ct);
    }

    public async Task<(List<JournalEntry> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var items = await _entries.GetPagedAsync(page, pageSize, ct);
        var total = await _entries.CountAsync(ct);
        return (items, total);
    }

    public Task<List<JournalEntry>> SearchAsync(
        string? query,
        DateOnly? from,
        DateOnly? to,
        List<int>? moodIds,
        List<int>? tagIds,
        CancellationToken ct = default)
        => _entries.SearchAsync(query, from, to, moodIds, tagIds, ct);

    private static List<EntryMood> BuildMoods(int primaryMoodId, List<int> secondaryMoodIds)
    {
        var list = new List<EntryMood>
        {
            new() { MoodId = primaryMoodId, IsPrimary = true }
        };

        foreach (var sid in secondaryMoodIds.Take(2))
            list.Add(new EntryMood { MoodId = sid, IsPrimary = false });

        return list;
    }

    private static int CountWords(string markdown)
    {
        var text = markdown
            .Replace("\r", " ")
            .Replace("\n", " ")
            .Trim();

        if (string.IsNullOrWhiteSpace(text)) return 0;

        // basic word count: split on whitespace
        return text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
