using System.Globalization;
using SchoolAccount.E2ETests.Common.Configuration;
using SchoolAccount.E2ETests.Common.Database;
using Shouldly;

namespace SchoolAccount.E2ETests.CollectData;

public class CollectDatabaseTests
{
    [Fact]
    public async Task Collect_portal_database_returns_rows()
    {
        // Act
        var rows = await CollectPortal()
            .SelectAsync("SELECT TOP (1) * FROM [dbo].[DataCollection]");

        // Assert
        rows.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task State_ledger_database_returns_rows()
    {
        // Arrange
        var stateLedger = StateLedger();
        var table = new CollectReturnStatusTable(stateLedger);
        var id = await table.AddAsync(CollectReturnStatusFaker.Generate());

        try
        {
            // Act
            var rows = await stateLedger.SelectAsync(
                "SELECT TOP (1) * FROM [dbo].[CollectReturnStatus]"
            );

            // Assert
            rows.ShouldNotBeEmpty();
        }
        finally
        {
            await table.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Added_status_row_can_be_read_back()
    {
        // Arrange
        var stateLedger = StateLedger();
        var table = new CollectReturnStatusTable(stateLedger);
        var row = CollectReturnStatusFaker.Generate();

        // Act
        var id = await table.AddAsync(row);

        try
        {
            var result = await stateLedger.SelectAsync(
                "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
                new { Id = id }
            );

            // Assert
            result.ShouldHaveSingleItem()["SchoolName"].ShouldBe(row.SchoolName);
        }
        finally
        {
            await table.RemoveAsync(id);
        }
    }

    [Fact]
    public async Task Removed_status_row_no_longer_exists()
    {
        // Arrange
        var stateLedger = StateLedger();
        var table = new CollectReturnStatusTable(stateLedger);
        var row = CollectReturnStatusFaker.Generate();
        var id = await table.AddAsync(row);

        // Act
        await table.RemoveAsync(id);

        var result = await stateLedger.SelectAsync(
            "SELECT * FROM [dbo].[CollectReturnStatus] WHERE [Id] = @Id",
            new { Id = id }
        );

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Sync_copies_a_portal_status_change_into_the_state_ledger()
    {
        // Arrange
        var collectPortal = CollectPortal();
        var stateLedger = StateLedger();
        var school = new
        {
            LAEStab = ReturnStatusSyncScripts.KnownLAEStab,
            CensusName = ReturnStatusSyncScripts.KnownCensusName,
        };

        var original = (
            await collectPortal.SelectAsync(ReturnStatusSyncScripts.DataReturnStatus, school)
        ).Single();
        var lastId = await LatestStatusIdAsync(stateLedger);

        var newStatus =
            Convert.ToInt32(original["Status"], CultureInfo.InvariantCulture) == 3 ? 2 : 3;
        var changed = new
        {
            school.LAEStab,
            school.CensusName,
            Status = newStatus,
            HighErrors = 1,
            LowErrors = 2,
            OKErrors = 3,
        };
        var addedSince = new
        {
            school.LAEStab,
            school.CensusName,
            LastId = lastId,
        };

        try
        {
            await collectPortal.ExecuteNonQueryAsync(
                ReturnStatusSyncScripts.UpdateDataReturnStatus,
                changed
            );

            // Act
            await stateLedger.ExecuteNonQueryAsync(
                ReturnStatusSyncScripts.SyncReturnStatus,
                school
            );

            var addedRows = await stateLedger.SelectAsync(
                ReturnStatusSyncScripts.StatusRowsAddedSince,
                addedSince
            );

            // Assert
            addedRows.ShouldHaveSingleItem()["ReturnStatusCode"].ShouldBe(newStatus);
        }
        finally
        {
            await collectPortal.ExecuteNonQueryAsync(
                ReturnStatusSyncScripts.UpdateDataReturnStatus,
                new
                {
                    school.LAEStab,
                    school.CensusName,
                    Status = original["Status"],
                    HighErrors = original["HighErrors"],
                    LowErrors = original["LowErrors"],
                    OKErrors = original["OKErrors"],
                }
            );
            await stateLedger.ExecuteNonQueryAsync(
                ReturnStatusSyncScripts.RemoveStatusRowsAddedSince,
                addedSince
            );
        }
    }

    private static LocalDatabase CollectPortal()
    {
        return LocalDatabase.CollectPortal(ConfigurationLoader.LoadSettings().Database);
    }

    private static LocalDatabase StateLedger()
    {
        return LocalDatabase.CollectStateLedger(ConfigurationLoader.LoadSettings().Database);
    }

    private static async Task<long> LatestStatusIdAsync(LocalDatabase stateLedger)
    {
        var rows = await stateLedger.SelectAsync(ReturnStatusSyncScripts.LatestStatusId);
        return Convert.ToInt64(rows.Single()["Id"], CultureInfo.InvariantCulture);
    }
}
