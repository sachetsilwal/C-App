using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Services.Abstractions;

public interface IJournalService
{
    Task<JournalEntry?> GetEntryAsync(DateOnly date, CancellationToken ct = default);

    Task<JournalEntry> UpsertEntryAsync(
        DateOnly date,
        string title,
        string markdown,
        int? categoryId,
        int primaryMoodId,
        List<int> secondaryMoodIds,
        List<int> tagIds,
        List<string> customTags,
        CancellationToken ct = default);

    Task DeleteEntryAsync(DateOnly date, CancellationToken ct = default);

    Task<(List<JournalEntry> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

    Task<List<JournalEntry>> SearchAsync(
        string? query,
        DateOnly? from,
        DateOnly? to,
        List<int>? moodIds,
        List<int>? tagIds,
        CancellationToken ct = default);
}
