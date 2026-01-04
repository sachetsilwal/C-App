using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Data;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Repositories;

public class TagRepository : ITagRepository
{
    private readonly JournalDbContext _db;
    public TagRepository(JournalDbContext db) => _db = db;

    public Task<List<Tag>> GetAllAsync(CancellationToken ct = default)
        => _db.Tags.AsNoTracking().OrderByDescending(t => t.IsPrebuilt).ThenBy(t => t.Name).ToListAsync(ct);

    public Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Tags.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Tag?> GetByNameAsync(string name, CancellationToken ct = default)
        => _db.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower(), ct);

    public async Task<Tag> CreateCustomAsync(string name, CancellationToken ct = default)
    {
        name = name.Trim();
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name cannot be empty.");

        var existing = await GetByNameAsync(name, ct);
        if (existing is not null) return existing;

        var tag = new Tag { Name = name, IsPrebuilt = false };
        await _db.Tags.AddAsync(tag, ct);
        await _db.SaveChangesAsync(ct);
        return tag;
    }
}
