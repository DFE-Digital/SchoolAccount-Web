namespace SchoolAccount.E2ETests.Common.Database;

/// <summary>
/// SQL for the return status sync test: change a school's status in COLLECTPortal, run the sync
/// in CollectStateLedger, then read the school's status history back from CollectStateLedger.
/// </summary>
public static class ReturnStatusSyncScripts
{
    /// <summary>LAESTAB of a school that exists in the restored COLLECTPortal backup.</summary>
    public const string KnownLAEStab = "8612009";

    /// <summary>A census that the school above has a data return for.</summary>
    public const string KnownCensusName = "SchoolCensus2025_Spring";

    /// <summary>Runs against COLLECTPortal.</summary>
    public const string UpdateDataReturnStatus = """
        UPDATE dr
        SET
          DRStatus = @Status,
          HighErrors = @HighErrors,
          LowErrors = @LowErrors,
          OKErrors = @OKErrors
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

    /// <summary>Runs against CollectStateLedger. Newest first.</summary>
    public const string ReturnStatusHistory = """
        SELECT [SchoolName]
              ,[LAEStab]
              ,[ReturnStatusCode]
              ,LAG([ReturnStatusCode]) OVER (ORDER BY [UpdatedAt] ASC) AS [PreviousReturnStatusCode]
              ,[Errors]
              ,[Queries]
              ,[OkdErrorsQueries]
              ,[Hash]
              ,[UpdatedAt]
              ,[Collection]
              ,[DCID]
        FROM [CollectStateLedger].[dbo].[CollectReturnStatus]
        WHERE LAEStab = @LAEStab AND [Collection] = @CensusName
        ORDER BY UpdatedAt DESC
        """;
}
