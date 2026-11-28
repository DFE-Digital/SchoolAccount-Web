using Aspire.Hosting.Dotnet;

namespace SchoolAccount.AppHost;

/// <summary>
/// SQL Server with COLLECTPortal and CollectStateLedger on it, built and filled with test data by
/// the Flyway folders under tests/SchoolAccount.E2ETests/Databases.
/// </summary>
internal static class CollectDatabases
{
    // Both use the iStore collation, so the ledger sync procedure's cross-database joins compare
    // strings the same way.
    private const string Collation = "Latin1_General_CI_AS";

    private const string FlywayImage = "flyway/flyway";
    private const string FlywayTag = "latest";

    public static CollectDatabaseResources AddCollectDatabases(
        this IDistributedApplicationBuilder builder
    )
    {
        var flywayRoot = Path.GetFullPath(
            Path.Combine(
                builder.AppHostDirectory,
                "..",
                "..",
                "tests",
                "SchoolAccount.E2ETests",
                "Databases"
            )
        );

        var sql = builder
            .AddSqlServer(ResourceNames.Sql)
            .WithLifetime(builder.GetDependencyLifetime())
            .WithDependencyGroup();

        var portal = AddDatabase(sql, ResourceNames.CollectPortal, "COLLECTPortal");
        var ledger = AddDatabase(sql, ResourceNames.CollectStateLedger, "CollectStateLedger");

        var portalMigrations = AddMigrations(
            builder,
            ResourceNames.PortalMigrations,
            sql,
            portal,
            Path.Combine(flywayRoot, "CollectPortal", "flyway"),
            ["conf", "migrations", "seed"]
        );

        var ledgerMigrations = AddMigrations(
            builder,
            ResourceNames.LedgerMigrations,
            sql,
            ledger,
            Path.Combine(flywayRoot, "CollectStateLedger", "flyway"),
            ["conf", "migrations", "repeatable", "seed"]
        );

        return new CollectDatabaseResources(sql, portalMigrations, ledgerMigrations);
    }

    /// <summary>
    /// Gives the Collect API the server's connection string once both databases are migrated and
    /// seeded. It has no database in it, because the API names its tables in full.
    /// </summary>
    public static IResourceBuilder<DotnetProjectResource> WithCollectDatabases(
        this IResourceBuilder<DotnetProjectResource> collectApi,
        CollectDatabaseResources databases
    )
    {
        return collectApi
            .WithEnvironment("Census__ConnectionString", databases.Sql)
            .WaitForCompletion(databases.PortalMigrations)
            .WaitForCompletion(databases.LedgerMigrations);
    }

    private static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(
        IResourceBuilder<SqlServerServerResource> sql,
        string resourceName,
        string databaseName
    )
    {
        return sql.AddDatabase(resourceName, databaseName)
            .WithCreationScript(
                $"""
                IF DB_ID(N'{databaseName}') IS NULL
                    CREATE DATABASE [{databaseName}] COLLATE {Collation};
                """
            );
    }

    private static IResourceBuilder<ContainerResource> AddMigrations(
        IDistributedApplicationBuilder builder,
        string resourceName,
        IResourceBuilder<SqlServerServerResource> sql,
        IResourceBuilder<SqlServerDatabaseResource> database,
        string flywayDirectory,
        string[] folders
    )
    {
        var endpoint = sql.Resource.PrimaryEndpoint;

        var migrations = builder
            .AddContainer(resourceName, FlywayImage, FlywayTag)
            .WithDependencyGroup()
            .WithArgs("migrate")
            // conf/flyway.toml leaves the connection to these settings.
            .WithEnvironment(
                "FLYWAY_URL",
                ReferenceExpression.Create(
                    $"jdbc:sqlserver://{endpoint.Property(EndpointProperty.Host)}:{endpoint.Property(EndpointProperty.Port)};databaseName={database.Resource.DatabaseName};encrypt=true;trustServerCertificate=true;"
                )
            )
            .WithEnvironment("FLYWAY_USER", "sa")
            .WithEnvironment("FLYWAY_PASSWORD", sql.Resource.PasswordParameter)
            .WaitFor(database);

        foreach (var folder in folders)
        {
            migrations.WithBindMount(
                Path.Combine(flywayDirectory, folder),
                $"/flyway/{folder}",
                isReadOnly: true
            );
        }

        return migrations;
    }
}

/// <summary>What the Collect API needs from <see cref="CollectDatabases"/>.</summary>
internal sealed record CollectDatabaseResources(
    IResourceBuilder<SqlServerServerResource> Sql,
    IResourceBuilder<ContainerResource> PortalMigrations,
    IResourceBuilder<ContainerResource> LedgerMigrations
);
