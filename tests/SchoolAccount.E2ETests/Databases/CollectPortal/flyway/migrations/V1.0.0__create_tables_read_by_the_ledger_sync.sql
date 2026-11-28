-- Only the COLLECTPortal tables that CollectStateLedger's AddChangedCollectReturnStatus reads.
-- Columns, keys and unique indexes match the iStore copy, so data copied from a restored
-- backup fits as it is. The real database has no foreign keys between these tables either.

CREATE TABLE dbo.DataCollection
(
    [DCID]                              [int]            NOT NULL,
    [DCName]                            [nvarchar](64)   NOT NULL,
    [DCDescription]                     [nvarchar](1000) NULL,
    [DCLevel]                           [tinyint]        NULL,
    [DCType]                            [tinyint]        NULL,
    [DCStartDate]                       [datetime]       NULL,
    [DCEndDate]                         [datetime]       NULL,
    [DCOpenDate]                        [datetime]       NULL,
    [DCCloseDate]                       [datetime]       NULL,
    [DCDueDate]                         [datetime]       NULL,
    [DCBladeInstalled]                  [bit]            NULL,
    [DCBladeSQLServer]                  [nvarchar](128)  NULL,
    [DCBladeSQLDatabase]                [nvarchar](128)  NULL,
    [DCState]                           [tinyint]        NULL,
    [DCStatus]                          [tinyint]        NULL,
    [SourceCanEdit]                     [bit]            NULL,
    [SourceCanAdd]                      [bit]            NULL,
    [SourceCanDelete]                   [bit]            NULL,
    [SourceCanOKErrors]                 [bit]            NULL,
    [AgentCanEdit]                      [bit]            NULL,
    [AgentCanAdd]                       [bit]            NULL,
    [AgentCanDelete]                    [bit]            NULL,
    [AgentCanOKErrors]                  [bit]            NULL,
    [CollectorCanEdit]                  [bit]            NULL,
    [CollectorCanAdd]                   [bit]            NULL,
    [CollectorCanDelete]                [bit]            NULL,
    [CollectorCanOKErrors]              [bit]            NULL,
    [IsMultiLevelCollection]            [bit]            NULL,
    [IsMultiSystemCollection]           [bit]            NULL,
    [MatchingReconciliation]            [bit]            NULL,
    [FilenameRetained]                  [bit]            NULL,
    [SourceResubmissionsBlocked]        [bit]            NULL,
    [AgentResubmissionsBlocked]         [bit]            NULL,
    [AgentMultipleResubmissionsBlocked] [bit]            NULL,
    [UserStamp]                         [int]            NOT NULL,
    [DateStamp]                         [datetime]       NOT NULL,
    [AgentsHaveAdminRights]             [bit]            NOT NULL,
    [AsyncProcessing]                   [bit]            NULL,
    [SnapshotSubmit]                    [bit]            NOT NULL,
    [SnapshotApprove]                   [bit]            NOT NULL,
    [SnapshotAuthorise]                 [bit]            NOT NULL,
    [MatchingDecoupled]                 [bit]            NULL,
    [ReconciliationDecoupled]           [bit]            NULL,
    [YonYRequired]                      [bit]            NULL,
    [TimeStampRequired]                 [bit]            NULL,
    [DisableXMLExports]                 [bit]            NOT NULL,
    [AcademyExportAllowed]              [bit]            NOT NULL,
    [AcademyExportType]                 [nvarchar](128)  NULL,
    [RestrictSubmission]                [bit]            NOT NULL,
    [CollectionGroupID]                 [int]            NOT NULL,
    [RestrictApproval]                  [bit]            NOT NULL,
    [RestrictNotes]                     [bit]            NOT NULL,
    [RestrictUnApproval]                [bit]            NOT NULL,
    CONSTRAINT [PK_DataCollection] PRIMARY KEY CLUSTERED ([DCID])
);

CREATE TABLE dbo.Organisation
(
    [OrganisationID]           [int]           NOT NULL,
    [OrganisationTypeID]       [int]           NOT NULL,
    [OrganisationNativeTypeID] [int]           NULL,
    [OrganisationNativeID]     [nvarchar](50)  NOT NULL,
    [OrganisationName]         [nvarchar](255) NOT NULL,
    [OrganisationAddress1]     [nvarchar](35)  NOT NULL,
    [OrganisationAddress2]     [nvarchar](35)  NOT NULL,
    [OrganisationAddress3]     [nvarchar](35)  NULL,
    [OrganisationAddress4]     [nvarchar](35)  NULL,
    [OrganisationAddress5]     [nvarchar](35)  NULL,
    [OrganisationPostcode]     [nvarchar](8)   NOT NULL,
    [OrganisationTelephone]    [nvarchar](35)  NULL,
    [OrganisationFax]          [nvarchar](35)  NULL,
    [OrganisationEmail]        [nvarchar](255) NULL,
    [OrganisationWeb]          [nvarchar](255) NULL,
    [OrganisationAgentID]      [int]           NULL,
    [OrganisationState]        [tinyint]       NOT NULL,
    [AllowSourceUpdate]        [bit]           NOT NULL,
    [UserStamp]                [int]           NOT NULL,
    [DateStamp]                [datetime]      NOT NULL,
    [Academy]                  [bit]           NOT NULL,
    CONSTRAINT [PK_Organisation] PRIMARY KEY CLUSTERED ([OrganisationID])
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_Organisation]
    ON dbo.Organisation ([OrganisationTypeID], [OrganisationNativeID]);

CREATE TABLE dbo.OrganisationRole
(
    [OrganisationRoleID]    [int]      NOT NULL,
    [DCID]                  [int]      NOT NULL,
    [RoleType]              [tinyint]  NOT NULL,
    [OrganisationID]        [int]      NOT NULL,
    [RoleState]             [tinyint]  NOT NULL,
    [UserStamp]             [int]      NOT NULL,
    [DateStamp]             [datetime] NOT NULL,
    [OldOrganisationRoleID] [int]      NULL,
    CONSTRAINT [PK_OrganisationRole] PRIMARY KEY CLUSTERED ([OrganisationRoleID])
);

CREATE UNIQUE NONCLUSTERED INDEX [IX_OrganisationRole]
    ON dbo.OrganisationRole ([DCID], [RoleType], [OrganisationID]);

CREATE TABLE dbo.DataReturn
(
    [DataReturnID]             [int]          NOT NULL,
    [DCID]                     [int]          NOT NULL,
    [AgentOrganisationRoleID]  [int]          NULL,
    [SourceOrganisationRoleID] [int]          NOT NULL,
    [DRStatus]                 [tinyint]      NOT NULL,
    [DRControl]                [tinyint]      NOT NULL,
    [SubmittedDate]            [datetime]     NULL,
    [ApprovedDate]             [datetime]     NULL,
    [AuthorisedDate]           [datetime]     NULL,
    [DRState]                  [tinyint]      NOT NULL,
    [SourceQueueID]            [bigint]       NULL,
    [AgentQueueID]             [bigint]       NULL,
    [CollectorQueueID]         [bigint]       NULL,
    [CollectorAgentQueueID]    [bigint]       NULL,
    [LowErrors]                [int]          NULL,
    [MediumErrors]             [int]          NULL,
    [HighErrors]               [int]          NULL,
    [OKErrors]                 [int]          NULL,
    [ValidatingHost]           [nvarchar](50) NULL,
    [UserStamp]                [int]          NULL,
    [DateStamp]                [datetime]     NOT NULL,
    CONSTRAINT [PK_DataReturn] PRIMARY KEY CLUSTERED ([DataReturnID])
);
