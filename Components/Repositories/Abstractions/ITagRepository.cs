using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Repositories.Abstractions;

public interface ITagRepository
{
    Task<List<Tag>> GetAllAsync(CancellationToken ct = default);
    Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Tag?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<Tag> CreateCustomAsync(string name, CancellationToken ct = default);
}
