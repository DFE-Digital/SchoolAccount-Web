-- Taken from sql/stored-procedures.sql in SchoolAccount-CollectStateLedgerDatabase, without the
-- USE statement. COLLECTPortal isn't in this container, so the procedure is created but has
-- nothing to read until it runs on a server that has both databases.

CREATE OR ALTER PROCEDURE dbo.AddChangedCollectReturnStatus
    @DCBladeSQLDatabase NVARCHAR(128)
    AS
BEGIN
    SET NOCOUNT ON;
    
DECLARE @DCID INT
    
SELECT @DCID = DCID
FROM COLLECTPortal.dbo.DataCollection
WHERE DCBladeSQLDatabase = @DCBladeSQLDatabase

IF @DCID IS NULL
BEGIN
    PRINT 'No DataCollection record found for census: ' + ISNULL(@DCBladeSQLDatabase, 'NULL')
    RETURN
END
    
-- Only insert rows where the hash is different from the last updated row or where no existing row exists
INSERT INTO CollectStateLedger.dbo.CollectReturnStatus
(
    SchoolName,
    LAEStab,
    ReturnStatusCode,
    Errors,
    Queries,
    OkdErrorsQueries,
    Hash,
    UpdatedAt,
    Collection,
    DCID,
    DataReturnID
)
SELECT
    src.SchoolName,
    src.LAEStab,
    src.ReturnStatusCode,
    src.Errors,
    src.Queries,
    src.OkdErrorsQueries,
    src.Hash,
    src.UpdatedAt,
    @DCBladeSQLDatabase as Collection,
    src.DCID,
    src.DataReturnID
FROM
    (
        -- Calculate the current values
        SELECT
            o.OrganisationName AS SchoolName,
            o.OrganisationNativeID AS LAEStab,
            dr.DRStatus AS ReturnStatusCode,
            dr.HighErrors AS Errors,
            dr.LowErrors AS Queries,
            dr.OKErrors AS OkdErrorsQueries,
            CONVERT(VARCHAR(32), HASHBYTES('MD5',
                                           CONCAT(CAST(dr.DRStatus AS VARCHAR), '|',
                                                  CAST(dr.HighErrors AS VARCHAR), '|',
                                                  CAST(dr.LowErrors AS VARCHAR), '|',
                                                  CAST(dr.OKErrors AS VARCHAR))), 2) AS Hash,
            GETDATE() AS UpdatedAt,
            @DCID AS DCID,
            dr.DataReturnID
        FROM COLLECTPortal.dbo.Organisation o
                 INNER JOIN COLLECTPortal.dbo.OrganisationRole orol
                            ON orol.OrganisationID = o.OrganisationID
                 INNER JOIN COLLECTPortal.dbo.DataReturn dr
                            ON dr.SourceOrganisationRoleID = orol.OrganisationRoleID
                 INNER JOIN COLLECTPortal.dbo.OrganisationRole orol2
                            ON dr.AgentOrganisationRoleID = orol2.OrganisationRoleID
                 INNER JOIN COLLECTPortal.dbo.Organisation o2
                            ON orol2.OrganisationID = o2.OrganisationID
        WHERE dr.DCID = @DCID
    ) AS src
        LEFT JOIN
    (
        -- Get ONLY the latest row's hash for each LAEStab and DCID
        SELECT
            LAEStab,
            DCID,
            Hash,
            UpdatedAt,
            ROW_NUMBER() OVER (
                PARTITION BY LAEStab, DCID 
                ORDER BY UpdatedAt DESC, ID DESC
            ) AS rn
        FROM CollectStateLedger.dbo.CollectReturnStatus
        WHERE DCID = @DCID
    ) AS tgt
    ON tgt.LAEStab = src.LAEStab
    AND tgt.DCID = src.DCID
    AND tgt.rn = 1             -- Only the most recent row
    WHERE tgt.LAEStab IS NULL  -- No existing row exists
    OR tgt.Hash <> src.Hash    -- Hash differs from the latest row
END
GO