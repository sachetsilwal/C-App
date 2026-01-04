using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Data;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Repositories;

public class SettingsRepository : ISettingsRepository
{
    private readonly JournalDbContext _db;
    public SettingsRepository(JournalDbContext db) => _db = db;

    public async Task<AppSettings> GetAsync(CancellationToken ct = default)
    {
        var s = await _db.Settings.FirstOrDefaultAsync(x => x.Id == 1, ct);
        if (s is null)
        {
            s = new AppSettings { Id = 1, UpdatedAt = DateTime.UtcNow };
            _db.Settings.Add(s);
            await _db.SaveChangesAsync(ct);
        }
        return s;
    }

    public Task UpdateAsync(AppSettings settings, CancellationToken ct = default)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        _db.Settings.Update(settings);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}

