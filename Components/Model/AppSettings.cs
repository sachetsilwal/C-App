using MYMAUIAPP.Components.Model.Enums;

namespace MYMAUIAPP.Components.Model;

public class AppSettings
{
    public int Id { get; set; } = 1; // single row settings

    public ThemeMode ThemeMode { get; set; } = ThemeMode.Dark;

    // Security
    public bool IsLockEnabled { get; set; }
    public string? PinSalt { get; set; }
    public string? PinHash { get; set; }

    public DateTime UpdatedAt { get; set; }
}

