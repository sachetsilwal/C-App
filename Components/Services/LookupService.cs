using MYMAUIAPP.Components.Model;
using MYMAUIAPP.Components.Repositories.Abstractions;
using MYMAUIAPP.Components.Services.Abstractions;

namespace MYMAUIAPP.Components.Services;

public class LookupService : ILookupService
{
    private readonly IMoodRepository _moods;
    private readonly ITagRepository _tags;
    private readonly ICategoryRepository _cats;

    public LookupService(IMoodRepository moods, ITagRepository tags, ICategoryRepository cats)
    {
        _moods = moods;
        _tags = tags;
        _cats = cats;
    }

    public Task<List<Mood>> GetMoodsAsync(CancellationToken ct = default) => _moods.GetAllAsync(ct);
    public Task<List<Tag>> GetTagsAsync(CancellationToken ct = default) => _tags.GetAllAsync(ct);
    public Task<List<Category>> GetCategoriesAsync(CancellationToken ct = default) => _cats.GetAllAsync(ct);
}
