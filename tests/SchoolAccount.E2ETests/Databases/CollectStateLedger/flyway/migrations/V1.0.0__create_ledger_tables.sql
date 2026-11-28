-- Taken from sql/tables.sql in SchoolAccount-CollectStateLedgerDatabase, without the
-- database name, as Flyway is already connected to CollectStateLedger.

CREATE TABLE dbo.CollectReturnStatus
(
    [Id]               [int] IDENTITY (1,1) PRIMARY KEY,
    [SchoolName]       [nvarchar](250) NOT NULL,
    [LAEStab]          [nvarchar](50)  NOT NULL,
    [ReturnStatusCode] [int]           NOT NULL,
    [Errors]           [int]           NULL,
    [Queries]          [int]           NULL,
    [OkdErrorsQueries] [int]           NULL,
    [Hash]             [nvarchar](36)  NOT NULL,
    [UpdatedAt]        [datetime]      NOT NULL,
    [DCID]             [int]           NOT NULL,
    [Collection]       [nvarchar](128) NOT NULL,
    [DataReturnID]     [int]           NOT NULL
);

CREATE TABLE dbo.RegisteredUsers
(
    [Id]      [int] IDENTITY (1,1) PRIMARY KEY,
    [LAEStab] [nvarchar](50)  NOT NULL,
    [Email]   [nvarchar](250) NOT NULL
);

CREATE TABLE dbo.JobStatus
(
    [Id]      [int] IDENTITY (1,1) PRIMARY KEY,
    [Name]    [nvarchar](50) NOT NULL,
    [LastRun] [datetime]     NULL
);
