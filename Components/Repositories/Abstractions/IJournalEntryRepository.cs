using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Repositories.Abstractions;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<JournalEntry?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<List<JournalEntry>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);

    Task<List<JournalEntry>> SearchAsync(
        string? query,
        DateOnly? from,
        DateOnly? to,
        List<int>? moodIds,
        List<int>? tagIds,
        CancellationToken ct = default);

    Task<int> CountAsync(CancellationToken ct = default);

    Task AddAsync(JournalEntry entry, CancellationToken ct = default);
    Task UpdateAsync(JournalEntry entry, CancellationToken ct = default);
    Task DeleteAsync(JournalEntry entry, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}

