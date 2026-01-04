using MYMAUIAPP.Components.Repositories.Abstractions;
using MYMAUIAPP.Components.Model.Enums;

namespace MYMAUIAPP.Components.Services;

public class ThemeService
{
    private readonly ISettingsRepository _settings;

    public ThemeService(ISettingsRepository settings)
    {
        _settings = settings;
    }

    public async Task<ThemeMode> GetAsync()
        => (await _settings.GetAsync()).ThemeMode;

    public async Task ToggleAsync()
    {
        var s = await _settings.GetAsync();
        s.ThemeMode = s.ThemeMode == ThemeMode.Dark
            ? ThemeMode.Light
            : ThemeMode.Dark;

        await _settings.UpdateAsync(s);
        await _settings.SaveChangesAsync();
    }
}
