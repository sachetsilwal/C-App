using Microsoft.EntityFrameworkCore;
using MYMAUIAPP.Components.Data;
using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;

namespace MYMAUIAPP.Components.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly JournalDbContext _db;
    public CategoryRepository(JournalDbContext db) => _db = db;

    public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
        => _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
}
