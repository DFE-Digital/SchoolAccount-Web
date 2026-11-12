namespace SchoolAccount.E2ETests.Common.Database;

public static class CollectPortalScripts
{
    public const string KnownLAEStab = "8612009";
    public const string KnownCensusName = "SchoolCensus2025_Spring";

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

    public const string SyncReturnStatus =
        "EXEC dbo.AddChangedCollectReturnStatus @DCBladeSQLDatabase = @CensusName";

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
