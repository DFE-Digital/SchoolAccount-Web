-- Made-up addresses on example.com. Two users for some schools, as a school can have several.

IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8034104' AND Email = N'headteacher.8034104@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8034104', N'headteacher.8034104@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8034104' AND Email = N'data.manager.8034104@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8034104', N'data.manager.8034104@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8733067' AND Email = N'headteacher.8733067@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8733067', N'headteacher.8733067@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'3313006' AND Email = N'headteacher.3313006@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'3313006', N'headteacher.3313006@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9261112' AND Email = N'headteacher.9261112@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9261112', N'headteacher.9261112@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9261112' AND Email = N'data.manager.9261112@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9261112', N'data.manager.9261112@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'3813315' AND Email = N'headteacher.3813315@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'3813315', N'headteacher.3813315@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8302182' AND Email = N'headteacher.8302182@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8302182', N'headteacher.8302182@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9372025' AND Email = N'headteacher.9372025@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9372025', N'headteacher.9372025@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9372025' AND Email = N'data.manager.9372025@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9372025', N'data.manager.9372025@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8862030' AND Email = N'headteacher.8862030@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8862030', N'headteacher.8862030@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'3942038' AND Email = N'headteacher.3942038@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'3942038', N'headteacher.3942038@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9263120' AND Email = N'headteacher.9263120@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9263120', N'headteacher.9263120@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'9263120' AND Email = N'data.manager.9263120@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'9263120', N'data.manager.9263120@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'2123434' AND Email = N'headteacher.2123434@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'2123434', N'headteacher.2123434@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8652191' AND Email = N'headteacher.8652191@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8652191', N'headteacher.8652191@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8612009' AND Email = N'headteacher.8612009@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8612009', N'headteacher.8612009@example.com');
IF NOT EXISTS (SELECT 1 FROM dbo.RegisteredUsers WHERE LAEStab = N'8612009' AND Email = N'data.manager.8612009@example.com')
    INSERT INTO dbo.RegisteredUsers (LAEStab, Email) VALUES (N'8612009', N'data.manager.8612009@example.com');
