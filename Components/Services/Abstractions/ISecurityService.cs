namespace MYMAUIAPP.Components.Services.Abstractions;

public interface ISecurityService
{
    Task<bool> IsLockEnabledAsync(CancellationToken ct = default);
    Task EnablePinAsync(string pin, CancellationToken ct = default);
    Task DisableLockAsync(CancellationToken ct = default);
    Task<bool> VerifyPinAsync(string pin, CancellationToken ct = default);
}
