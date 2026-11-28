namespace SchoolAccount.E2ETests.Common.Database;

/// <summary>
/// SQL for the return status sync test: change a school's status in COLLECTPortal, run the sync
/// in CollectStateLedger, check the rows it added, then put both databases back as they were.
/// </summary>
public static class ReturnStatusSyncScripts
{
    /// <summary>LAESTAB of a school that exists in the restored COLLECTPortal backup.</summary>
    public const string KnownLAEStab = "8612009";

    /// <summary>A census that the school above has a data return for.</summary>
    public const string KnownCensusName = "SchoolCensus2025_Spring";

    /// <summary>Runs against COLLECTPortal. The school's current status, so it can be put back.</summary>
    public const string DataReturnStatus = $"""
        SELECT dr.DRStatus AS [Status], dr.HighErrors, dr.LowErrors, dr.OKErrors
        {_schoolDataReturn}
        """;

    /// <summary>Runs against COLLECTPortal.</summary>
    public const string UpdateDataReturnStatus = $"""
        UPDATE dr
        SET
          DRStatus = @Status,
          HighErrors = @HighErrors,
          LowErrors = @LowErrors,
          OKErrors = @OKErrors
        {_schoolDataReturn}
        """;

    private const string _schoolDataReturn = """
        FROM COLLECTPortal.dbo.DataReturn dr
        INNER JOIN COLLECTPortal.dbo.OrganisationRole orol
            ON dr.SourceOrganisationRoleID = orol.OrganisationRoleID
        INNER JOIN COLLECTPortal.dbo.Organisation o
            ON orol.OrganisationID = o.OrganisationID
        INNER JOIN COLLECTPortal.dbo.OrganisationRole orol2
            ON dr.AgentOrganisationRoleID = orol2.OrganisationRoleID
        INNER JOIN COLLECTPortal.dbo.Organisation o2
            ON orol2.OrganisationID = o2.OrganisationID
        INNER JOIN COLLECTPortal.dbo.DataCollection dc
            ON dc.DCID = dr.DCID
        WHERE o.OrganisationNativeID = @LAEStab
        AND dc.DCBladeSQLDatabase = @CensusName
        """;

    /// <summary>Runs against CollectStateLedger.</summary>
    public const string SyncReturnStatus =
        "EXEC dbo.AddChangedCollectReturnStatus @DCBladeSQLDatabase = @CensusName";

    /// <summary>
    /// Runs against CollectStateLedger. The Id of the newest status row, or 0 when there are none,
    /// so the test can tell which rows the sync adds.
    /// </summary>
    public const string LatestStatusId =
        "SELECT ISNULL(MAX([Id]), 0) AS [Id] FROM [CollectStateLedger].[dbo].[CollectReturnStatus]";

    /// <summary>Runs against CollectStateLedger. The school's status rows added after @LastId.</summary>
    public const string StatusRowsAddedSince = """
        SELECT [Id], [ReturnStatusCode]
        FROM [CollectStateLedger].[dbo].[CollectReturnStatus]
        WHERE [Id] > @LastId AND LAEStab = @LAEStab AND [Collection] = @CensusName
        """;

    /// <summary>
    /// Runs against CollectStateLedger. Removes every row for the census added after @LastId,
    /// for any school, because the sync covers the whole census.
    /// </summary>
    public const string RemoveStatusRowsAddedSince = """
        DELETE FROM [CollectStateLedger].[dbo].[CollectReturnStatus]
        WHERE [Id] > @LastId AND [Collection] = @CensusName
        """;
}
