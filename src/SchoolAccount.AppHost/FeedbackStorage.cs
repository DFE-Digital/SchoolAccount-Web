using Aspire.Hosting.Azure;
using Aspire.Hosting.Dotnet;
using Azure.Data.Tables;

namespace SchoolAccount.AppHost;

/// <summary>
/// Azure Table Storage for the feedback form, run in the Azurite emulator on free ports, so it
/// doesn't clash with the Azurite from SchoolAccount-LocalDevTools on 10000 to 10002.
/// </summary>
internal static class FeedbackStorage
{
    // The table the web app writes feedback to (SchoolAccount.Infrastructure's AddAzureTableClient).
    // The app doesn't create it, so the AppHost does once the emulator is up.
    private const string FeedbackTable = "SchoolAccountFeedback";

    public static IResourceBuilder<AzureTableStorageResource> AddFeedbackStorage(
        this IDistributedApplicationBuilder builder
    )
    {
        return builder
            .AddAzureStorage(ResourceNames.Storage)
            .RunAsEmulator(emulator =>
                emulator.WithLifetime(builder.GetDependencyLifetime()).WithDependencyGroup()
            )
            .AddTables(ResourceNames.Tables)
            .OnResourceReady(
                static async (tables, _, cancellationToken) =>
                {
                    var connectionString = await tables.ConnectionStringExpression.GetValueAsync(
                        cancellationToken
                    );

                    await new TableServiceClient(connectionString).CreateTableIfNotExistsAsync(
                        FeedbackTable,
                        cancellationToken
                    );
                }
            );
    }

    public static IResourceBuilder<DotnetProjectResource> WithFeedbackStorage(
        this IResourceBuilder<DotnetProjectResource> web,
        IResourceBuilder<AzureTableStorageResource> tables
    )
    {
        return web.WithEnvironment("Azure__TableStorageConnectionString", tables).WaitFor(tables);
    }
}
