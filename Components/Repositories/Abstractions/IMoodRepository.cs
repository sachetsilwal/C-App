using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Repositories.Abstractions;

public interface IMoodRepository
{
    Task<List<Mood>> GetAllAsync(CancellationToken ct = default);
    Task<Mood?> GetByIdAsync(int id, CancellationToken ct = default);
}
