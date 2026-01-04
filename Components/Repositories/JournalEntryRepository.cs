using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Data;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Repositories;

public class JournalEntryRepository : IJournalEntryRepository
{
    private readonly JournalDbContext _db;

    public JournalEntryRepository(JournalDbContext db) => _db = db;

    public Task<JournalEntry?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => _db.JournalEntries
            .Include(e => e.Category)
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.EntryDate == date, ct);

    public Task<JournalEntry?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.JournalEntries
            .Include(e => e.Category)
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<List<JournalEntry>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        return await _db.JournalEntries
            .AsNoTracking()
            .Include(e => e.Category)
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .OrderByDescending(e => e.EntryDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<List<JournalEntry>> SearchAsync(
        string? query,
        DateOnly? from,
        DateOnly? to,
        List<int>? moodIds,
        List<int>? tagIds,
        CancellationToken ct = default)
    {
        var q = _db.JournalEntries
            .Include(e => e.Category)
            .Include(e => e.EntryMoods).ThenInclude(em => em.Mood)
            .Include(e => e.EntryTags).ThenInclude(et => et.Tag)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim();
            q = q.Where(e => e.Title.Contains(term) || e.ContentMarkdown.Contains(term));
        }

        if (from is not null) q = q.Where(e => e.EntryDate >= from.Value);
        if (to is not null) q = q.Where(e => e.EntryDate <= to.Value);

        if (moodIds is { Count: > 0 })
        {
            q = q.Where(e => e.EntryMoods.Any(em => moodIds.Contains(em.MoodId)));
        }

        if (tagIds is { Count: > 0 })
        {
            q = q.Where(e => e.EntryTags.Any(et => tagIds.Contains(et.TagId)));
        }

        return await q
            .AsNoTracking()
            .OrderByDescending(e => e.EntryDate)
            .ToListAsync(ct);
    }

    public Task<int> CountAsync(CancellationToken ct = default)
        => _db.JournalEntries.CountAsync(ct);

    public Task AddAsync(JournalEntry entry, CancellationToken ct = default)
        => _db.JournalEntries.AddAsync(entry, ct).AsTask();

    public Task UpdateAsync(JournalEntry entry, CancellationToken ct = default)
    {
        _db.JournalEntries.Update(entry);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(JournalEntry entry, CancellationToken ct = default)
    {
        _db.JournalEntries.Remove(entry);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
