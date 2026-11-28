-- =============================================================================
-- Add Return Status
-- Writes one ledger row the way AddChangedCollectReturnStatus does, including
-- its MD5 hash of the status and counts. UpdatedAt is @MinutesAgo before @Now.
-- =============================================================================

CREATE OR ALTER PROCEDURE #AddReturnStatus @SchoolName NVARCHAR(250),
                                           @LAEStab NVARCHAR(50),
                                           @ReturnStatusCode INT,
                                           @Errors INT,
                                           @Queries INT,
                                           @OkdErrorsQueries INT,
                                           @Now DATETIME,
                                           @MinutesAgo INT,
                                           @DCID INT,
                                           @Collection NVARCHAR(128),
                                           @DataReturnID INT
AS
BEGIN
    INSERT INTO dbo.CollectReturnStatus (SchoolName, LAEStab, ReturnStatusCode, Errors, Queries,
                                         OkdErrorsQueries, Hash, UpdatedAt, DCID, Collection,
                                         DataReturnID)
    VALUES (@SchoolName, @LAEStab, @ReturnStatusCode, @Errors, @Queries, @OkdErrorsQueries,
            CONVERT(VARCHAR(32), HASHBYTES('MD5',
                                           CONCAT(CAST(@ReturnStatusCode AS VARCHAR), '|',
                                                  CAST(@Errors AS VARCHAR), '|',
                                                  CAST(@Queries AS VARCHAR), '|',
                                                  CAST(@OkdErrorsQueries AS VARCHAR))), 2),
            DATEADD(MINUTE, -@MinutesAgo, @Now), @DCID, @Collection, @DataReturnID);
END
GO
