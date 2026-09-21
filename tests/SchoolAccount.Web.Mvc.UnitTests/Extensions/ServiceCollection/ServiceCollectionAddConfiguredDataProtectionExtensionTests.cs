using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SchoolAccount.Web.Mvc.Hosting.Extensions;
using SchoolAccount.Web.Mvc.Hosting.Models;
using Shouldly;

namespace SchoolAccount.Web.Mvc.UnitTests.Extensions.ServiceCollection;

public class ServiceCollectionAddConfiguredDataProtectionExtensionTests
{
    private static ConfigurationManager BuildConfiguration(
        string? keyRingBlobUri = null,
        string? keyEncryptionKeyUri = null
    )
    {
        var configManager = new ConfigurationManager();
        configManager.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [$"{DataProtectionSettings.SectionName}:KeyRingBlobUri"] = keyRingBlobUri,
                [$"{DataProtectionSettings.SectionName}:KeyEncryptionKeyUri"] = keyEncryptionKeyUri,
            }
        );

        return configManager;
    }

    [Fact]
    public void Persists_the_key_ring_to_blob_storage_when_configured()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(
            "https://example.blob.core.windows.net/keys/schoolaccount.xml",
            "https://example.vault.azure.net/keys/data-protection"
        );
        services.AddConfiguredDataProtection(configuration);

        var provider = services.BuildServiceProvider();

        // Act
        var options = provider.GetRequiredService<IOptions<KeyManagementOptions>>().Value;

        // Assert
        options.XmlRepository.ShouldNotBeNull();
        options.XmlRepository.GetType().Name.ShouldBe("AzureBlobXmlRepository");
        options.XmlEncryptor.ShouldNotBeNull();
        options.XmlEncryptor.GetType().Name.ShouldBe("AzureKeyVaultXmlEncryptor");
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("https://example.blob.core.windows.net/keys/schoolaccount.xml", null)]
    [InlineData(null, "https://example.vault.azure.net/keys/data-protection")]
    [InlineData("", "")]
    public void Leaves_the_key_ring_alone_when_the_blob_or_key_is_missing(
        string? keyRingBlobUri,
        string? keyEncryptionKeyUri
    )
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        using var configuration = BuildConfiguration(keyRingBlobUri, keyEncryptionKeyUri);

        // Act
        services.AddConfiguredDataProtection(configuration);

        // Assert - nothing registered, so the framework's own defaults apply
        services.ShouldBeEmpty();
    }
}
