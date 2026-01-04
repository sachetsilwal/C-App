using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Data;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Repositories;

public class MoodRepository : IMoodRepository
{
    private readonly JournalDbContext _db;
    public MoodRepository(JournalDbContext db) => _db = db;

    public Task<List<Mood>> GetAllAsync(CancellationToken ct = default)
        => _db.Moods.AsNoTracking().OrderBy(m => m.MoodGroup).ThenBy(m => m.Name).ToListAsync(ct);

    public Task<Mood?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Moods.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, ct);
}
