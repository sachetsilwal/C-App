using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Repositories.Abstractions;

public interface ISettingsRepository
{
    Task<AppSettings> GetAsync(CancellationToken ct = default);
    Task UpdateAsync(AppSettings settings, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
