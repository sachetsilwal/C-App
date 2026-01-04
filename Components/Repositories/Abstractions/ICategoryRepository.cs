using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Repositories.Abstractions;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);
}
