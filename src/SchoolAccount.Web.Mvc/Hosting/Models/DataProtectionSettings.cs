namespace SchoolAccount.Web.Mvc.Hosting.Models;

public class DataProtectionSettings
{
    public const string SectionName = "DataProtectionSettings";

    /// <summary>
    /// The application name the key ring is isolated by. Payloads are only readable by apps
    /// sharing this name, so it must stay stable across deployments the default is
    /// derived from the content root path, which is not stable.
    /// </summary>
    public const string ApplicationName = "SchoolAccount.Web";

    /// <summary>
    /// Blob holding the key ring, e.g. "https://example.blob.core.windows.net/keys/schoolaccount.xml".
    /// Leave empty to fall back to a local only key ring.
    /// </summary>
    public string? KeyRingBlobUri { get; init; }

    /// <summary>
    /// Key Vault key the key ring is encrypted with, e.g.
    /// "https://example.vault.azure.net/keys/data-protection". Leave empty to fall back to the
    /// a local key ring.
    /// </summary>
    public string? KeyEncryptionKeyUri { get; init; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(KeyRingBlobUri)
        && !string.IsNullOrWhiteSpace(KeyEncryptionKeyUri);
}
