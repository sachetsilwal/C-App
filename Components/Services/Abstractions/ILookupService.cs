using MYMAUIAPP.Components.Model;

namespace MYMAUIAPP.Components.Services.Abstractions;

public interface ILookupService
{
    Task<List<Mood>> GetMoodsAsync(CancellationToken ct = default);
    Task<List<Tag>> GetTagsAsync(CancellationToken ct = default);
    Task<List<Category>> GetCategoriesAsync(CancellationToken ct = default);
}
