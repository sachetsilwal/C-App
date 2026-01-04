using System.Security.Cryptography;
using System.Text;
using MYMAUIAPP.Components.Repositories.Abstractions;
using MYMAUIAPP.Components.Services.Abstractions;

namespace MYMAUIAPP.Components.Services;

public class SecurityService : ISecurityService
{
    private readonly ISettingsRepository _settings;

    public SecurityService(ISettingsRepository settings) => _settings = settings;

    public async Task<bool> IsLockEnabledAsync(CancellationToken ct = default)
    {
        var s = await _settings.GetAsync(ct);
        return s.IsLockEnabled && !string.IsNullOrWhiteSpace(s.PinHash) && !string.IsNullOrWhiteSpace(s.PinSalt);
    }

    public async Task EnablePinAsync(string pin, CancellationToken ct = default)
    {
        pin = (pin ?? "").Trim();
        if (pin.Length < 4 || pin.Length > 12 || !pin.All(char.IsDigit))
            throw new ArgumentException("PIN must be 4–12 digits.");

        var salt = CreateSalt();
        var hash = HashPin(pin, salt);

        var s = await _settings.GetAsync(ct);
        s.IsLockEnabled = true;
        s.PinSalt = salt;
        s.PinHash = hash;

        await _settings.UpdateAsync(s, ct);
        await _settings.SaveChangesAsync(ct);
    }

    public async Task DisableLockAsync(CancellationToken ct = default)
    {
        var s = await _settings.GetAsync(ct);
        s.IsLockEnabled = false;
        s.PinSalt = null;
        s.PinHash = null;

        await _settings.UpdateAsync(s, ct);
        await _settings.SaveChangesAsync(ct);
    }

    public async Task<bool> VerifyPinAsync(string pin, CancellationToken ct = default)
    {
        var s = await _settings.GetAsync(ct);
        if (!s.IsLockEnabled || string.IsNullOrWhiteSpace(s.PinSalt) || string.IsNullOrWhiteSpace(s.PinHash))
            return true; // no lock enabled

        var candidate = HashPin((pin ?? "").Trim(), s.PinSalt);
        return FixedTimeEquals(candidate, s.PinHash);
    }

    private static string CreateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(bytes);
    }

    private static string HashPin(string pin, string saltBase64)
    {
        var salt = Convert.FromBase64String(saltBase64);

        // PBKDF2-HMACSHA256
        var pbkdf2 = new Rfc2898DeriveBytes(pin, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }

    private static bool FixedTimeEquals(string aBase64, string bBase64)
    {
        var a = Convert.FromBase64String(aBase64);
        var b = Convert.FromBase64String(bBase64);
        return CryptographicOperations.FixedTimeEquals(a, b);
    }
}
