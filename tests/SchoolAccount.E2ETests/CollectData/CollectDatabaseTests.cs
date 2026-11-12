using SchoolAccount.E2ETests.Common.Configuration;
using SchoolAccount.E2ETests.Common.Database;

namespace SchoolAccount.E2ETests.CollectData;

public class CollectDatabaseTests
{
    private readonly TestDatabase _collectPortal;
    private readonly TestDatabase _stateLedger;
    private readonly CollectReturnStatusTable _table;

    public CollectDatabaseTests()
    {
        var settings = ConfigurationLoader.LoadSettings();
        _collectPortal = TestDatabases.CollectPortal(settings.Database!);
        _stateLedger = TestDatabases.CollectStateLedger(settings.Database!);
        _table = new CollectReturnStatusTable(_stateLedger);
    }

    [Fact]
    public async Task Collect_portal_and_state_ledger_databases_both_return_rows()
    {
        var portalRows = await _collectPortal.SelectAsync(
            "SELECT TOP (1) * FROM [dbo].[DataCollection]"
        );
        var ledgerRows = await _stateLedger.SelectAsync(
            "SELECT TOP (1) * FROM [dbo].[CollectReturnStatus]"
        );

        Assert.NotEmpty(portalRows);
        Assert.NotEmpty(ledgerRows);
    }

    [Fact]
    public async Task Added_status_row_can_be_read_back()
    {
        var row = CollectReturnStatusFaker.Generate();
        var id = await _table.AddAsync(row);

        try
        {
            var result = await _stateLedger.SelectAsync(
                "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
                new { Id = id }
            );

            Assert.Single(result);
            Assert.Equal(row.SchoolName, result[0]["SchoolName"]);
        }
        finally
        {
            await _table.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Removed_status_row_no_longer_exists()
    {
        var row = CollectReturnStatusFaker.Generate();
        var id = await _table.AddAsync(row);

        await _table.RemoveAsync(id);

        var result = await _stateLedger.SelectAsync(
            "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
            new { Id = id }
        );

        Assert.Empty(result);
    }

    [Fact]
    public async Task Sync_copies_a_portal_status_change_into_the_state_ledger()
    {
        var parameters = new
        {
            LAEStab = CollectPortalScripts.KnownLAEStab,
            CensusName = CollectPortalScripts.KnownCensusName,
            Status = 3,
            HighErrors = 1,
            LowErrors = 2,
            OKErrors = 3,
        };

        await _collectPortal.ExecuteNonQueryAsync(
            CollectPortalScripts.UpdateDataReturnStatus,
            parameters
        );
        await _stateLedger.ExecuteNonQueryAsync(CollectPortalScripts.SyncReturnStatus, parameters);

        var history = await _stateLedger.SelectAsync(
            CollectPortalScripts.ReturnStatusHistory,
            parameters
        );

        Assert.NotEmpty(history);
        Assert.Equal(parameters.Status, history[0]["ReturnStatusCode"]);
    }
}
