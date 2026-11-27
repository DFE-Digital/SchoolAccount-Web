using SchoolAccount.E2ETests.Common.Configuration;
using SchoolAccount.E2ETests.Common.Database;

namespace SchoolAccount.E2ETests.CollectData;

public class CollectDatabaseTests
{
    [Fact]
    public async Task Collect_portal_and_state_ledger_databases_both_return_rows()
    {
        var portalRows = await CollectPortal()
            .SelectAsync("SELECT TOP (1) * FROM [dbo].[DataCollection]");
        var ledgerRows = await StateLedger()
            .SelectAsync("SELECT TOP (1) * FROM [dbo].[CollectReturnStatus]");

        Assert.NotEmpty(portalRows);
        Assert.NotEmpty(ledgerRows);
    }

    [Fact]
    public async Task Added_status_row_can_be_read_back()
    {
        var stateLedger = StateLedger();
        var table = new CollectReturnStatusTable(stateLedger);
        var row = CollectReturnStatusFaker.Generate();
        var id = await table.AddAsync(row);

        try
        {
            var result = await stateLedger.SelectAsync(
                "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
                new { Id = id }
            );

            Assert.Single(result);
            Assert.Equal(row.SchoolName, result[0]["SchoolName"]);
        }
        finally
        {
            await table.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Removed_status_row_no_longer_exists()
    {
        var stateLedger = StateLedger();
        var table = new CollectReturnStatusTable(stateLedger);
        var row = CollectReturnStatusFaker.Generate();
        var id = await table.AddAsync(row);

        await table.RemoveAsync(id);

        var result = await stateLedger.SelectAsync(
            "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
            new { Id = id }
        );

        Assert.Empty(result);
    }

    [Fact]
    public async Task Sync_copies_a_portal_status_change_into_the_state_ledger()
    {
        var collectPortal = CollectPortal();
        var stateLedger = StateLedger();
        var parameters = new
        {
            LAEStab = ReturnStatusSyncScripts.KnownLAEStab,
            CensusName = ReturnStatusSyncScripts.KnownCensusName,
            Status = 3,
            HighErrors = 1,
            LowErrors = 2,
            OKErrors = 3,
        };

        await collectPortal.ExecuteNonQueryAsync(
            ReturnStatusSyncScripts.UpdateDataReturnStatus,
            parameters
        );
        await stateLedger.ExecuteNonQueryAsync(
            ReturnStatusSyncScripts.SyncReturnStatus,
            parameters
        );

        var history = await stateLedger.SelectAsync(
            ReturnStatusSyncScripts.ReturnStatusHistory,
            parameters
        );

        Assert.NotEmpty(history);
        Assert.Equal(parameters.Status, history[0]["ReturnStatusCode"]);
    }

    private static LocalDatabase CollectPortal()
    {
        return LocalDatabase.CollectPortal(ConfigurationLoader.LoadSettings().Database);
    }

    private static LocalDatabase StateLedger()
    {
        return LocalDatabase.CollectStateLedger(ConfigurationLoader.LoadSettings().Database);
    }
}
