using System.Globalization;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Data.SqlClient;
using SchoolAccount.AppHost;
using SchoolAccount.E2ETests.Common.Configuration;

[assembly: AssemblyFixture(typeof(SchoolAccount.E2ETests.Common.Fixtures.AspireFixture))]

namespace SchoolAccount.E2ETests.Common.Fixtures;

/// <summary>
/// Starts the Aspire AppHost once for the whole run: SQL Server with both Collect databases
/// seeded, the Collect API and the web app. Then points the suite at them, ahead of any test or
/// other fixture reading its settings. Everything is stopped when the run ends.
/// </summary>
public sealed class AspireFixture : IAsyncLifetime
{
    private static readonly TimeSpan _startTimeout = TimeSpan.FromMinutes(5);

    // In start-up order. Waiting on each in turn means a failure names the resource at fault,
    // instead of the run waiting on the web app until it times out.
    private static readonly string[] _resources =
    [
        ResourceNames.PortalMigrations,
        ResourceNames.LedgerMigrations,
        ResourceNames.CollectApi,
        ResourceNames.Web,
    ];

    private DistributedApplication? _app;

    public async ValueTask InitializeAsync()
    {
        if (!ConfigurationLoader.LoadSettings().Aspire.Enabled)
        {
            return;
        }

        // Stopping the run, for example from the IDE, also stops start-up.
        var runCancelled = TestContext.Current.CancellationToken;
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(runCancelled);
        timeout.CancelAfter(_startTimeout);

        try
        {
            await StartAsync(timeout.Token);
        }
        catch (OperationCanceledException exception)
            when (timeout.IsCancellationRequested && !runCancelled.IsCancellationRequested)
        {
            await DisposeAsync();
            throw new TimeoutException(
                $"The Aspire AppHost didn't start within {_startTimeout.TotalMinutes} minutes. "
                    + "Check Docker is running.",
                exception
            );
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    private async Task StartAsync(CancellationToken cancellationToken)
    {
        // The testing builder gives every resource a random port by default. The web app has to
        // keep https://127.0.0.1:7041, as that's the only address DfE Sign-in sends users back to.
        // The AppHost keeps its containers running between runs by default; a test run gets its
        // own, removed when it ends, so it always starts from clean data.
        var appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.SchoolAccount_AppHost>(
                ["DcpPublisher:RandomizePorts=false", "PersistentContainers=false"],
                cancellationToken
            );

        _app = await appHost.BuildAsync(cancellationToken);
        await _app.StartAsync(cancellationToken);

        foreach (var resource in _resources)
        {
            await WaitForResourceAsync(_app, resource, cancellationToken);
        }

        var sql = new SqlConnectionStringBuilder(
            await _app.GetConnectionStringAsync(ResourceNames.Sql, cancellationToken)
        );

        ConfigurationLoader.Override(
            new Dictionary<string, string?>
            {
                ["SchoolAccount:Url"] = _app.GetEndpoint(ResourceNames.Web, "https").ToString(),
                ["Database:Server"] = sql.DataSource,
                ["Database:UserId"] = sql.UserID,
                ["Database:Password"] = sql.Password,
            }
        );
    }

    // The migrations finish and exit; everything else stays up and reports healthy. Either way,
    // a resource that fails, such as when Docker isn't running, stops the wait straight away.
    private static async Task WaitForResourceAsync(
        DistributedApplication app,
        string resource,
        CancellationToken cancellationToken
    )
    {
        if (resource is ResourceNames.PortalMigrations or ResourceNames.LedgerMigrations)
        {
            var ended = await app.ResourceNotifications.WaitForResourceAsync(
                resource,
                resourceEvent =>
                    KnownResourceStates.TerminalStates.Contains(resourceEvent.Snapshot.State?.Text),
                cancellationToken
            );

            var exitCode = ended.Snapshot.ExitCode;

            if (exitCode != 0)
            {
                throw new InvalidOperationException(
                    $"{resource} ended as {ended.Snapshot.State?.Text} with exit code "
                        + $"{exitCode?.ToString(CultureInfo.InvariantCulture) ?? "unknown"}. "
                        + "Its logs are in the test output."
                );
            }

            return;
        }

        await app.ResourceNotifications.WaitForResourceHealthyAsync(
            resource,
            WaitBehavior.StopOnResourceUnavailable,
            cancellationToken
        );
    }

    // Safe to call more than once: a failed start disposes straight away, and xUnit disposes
    // again at the end of the run.
    public async ValueTask DisposeAsync()
    {
        var app = Interlocked.Exchange(ref _app, null);

        if (app is not null)
        {
            await app.DisposeAsync();
        }
    }
}
