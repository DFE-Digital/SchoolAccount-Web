-- The CollectNotifications job reads its last run time from here, keyed by its Census:JobName
-- ("collect-notifications-local" in its Development settings). A day ago means the recent
-- changes in the Spring 2025 history are picked up on its next run.

IF NOT EXISTS (SELECT 1 FROM dbo.JobStatus WHERE Name = N'collect-notifications-local')
    INSERT INTO dbo.JobStatus (Name, LastRun)
    VALUES (N'collect-notifications-local', DATEADD(DAY, -1, GETUTCDATE()));
