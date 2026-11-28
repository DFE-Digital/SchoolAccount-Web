-- The schools that developers' accounts are linked to in the DfE Sign-in test environment, so
-- whichever one you sign in as has census data. Only the establishments: the trusts on those
-- accounts have no census return. LAEStabs, names and Spring 2025 rows come from a restored
-- COLLECTPortal backup. East Herrington Primary Academy (URN 138331) is the school the E2E tests
-- sign in as, so it also has a full history in both censuses.
--
-- In SchoolCensus2026_Autumn each school is at a different status, so the journey page can be
-- checked at every stage by signing in as a different organisation. Those rows are made up, with
-- DataReturnIDs from 7500100.
--
-- A school that already has rows for the census is left alone, so re-running migrate adds nothing.

DECLARE @Now DATETIME = GETUTCDATE();

-- Abbey Manor College (2091103)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'2091103' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Abbey Manor College', N'2091103', 7, 0, 1, 3, @Now, 11520, 1172, N'SchoolCensus2025_Spring', 7391139;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'2091103' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Abbey Manor College', N'2091103', 1, 0, 0, 0, @Now, 30, 1205, N'SchoolCensus2026_Autumn', 7500100;
END;

-- Bede Academy (9296905)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9296905' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bede Academy', N'9296905', 9, 0, 1, 13, @Now, 11617, 1172, N'SchoolCensus2025_Spring', 7394324;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9296905' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Bede Academy', N'9296905', 23, 0, 0, 0, @Now, 71, 1205, N'SchoolCensus2026_Autumn', 7500101;
END;

-- Beulah Junior School (3062003)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3062003' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Beulah Junior School', N'3062003', 7, 0, 1, 3, @Now, 11714, 1172, N'SchoolCensus2025_Spring', 7388117;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3062003' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Beulah Junior School', N'3062003', 31, 0, 0, 0, @Now, 112, 1205, N'SchoolCensus2026_Autumn', 7500102;
END;

-- Bourne Abbey Church of England Primary Academy (9253510)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9253510' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bourne Abbey Church of England Primary Academy', N'9253510', 9, 0, 0, 6, @Now, 11811, 1172, N'SchoolCensus2025_Spring', 7394286;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9253510' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Bourne Abbey Church of England Primary Academy', N'9253510', 32, 0, 0, 0, @Now, 153, 1205, N'SchoolCensus2026_Autumn', 7500103;
END;

-- Brookland Infant and Nursery School (3022008)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3022008' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Brookland Infant and Nursery School', N'3022008', 9, 0, 1, 4, @Now, 11908, 1172, N'SchoolCensus2025_Spring', 7393838;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3022008' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Brookland Infant and Nursery School', N'3022008', 13, 0, 0, 0, @Now, 194, 1205, N'SchoolCensus2026_Autumn', 7500104;
END;

-- Colsterworth Church of England Primary School (9252049)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9252049' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Colsterworth Church of England Primary School', N'9252049', 16, 0, 0, 4, @Now, 12005, 1172, N'SchoolCensus2025_Spring', 7401514;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9252049' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Colsterworth Church of England Primary School', N'9252049', 12, 0, 0, 0, @Now, 235, 1205, N'SchoolCensus2026_Autumn', 7500105;
END;

-- East Herrington Primary Academy (3942091)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3942091' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 1, 0, 0, 0, @Now, 34560, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 31, 0, 0, 0, @Now, 33120, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 11, 6, 4, 0, @Now, 33060, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 31, 0, 0, 0, @Now, 31680, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 2, 0, 3, 0, @Now, 31620, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 4, 0, 1, 7, @Now, 30240, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 7, 0, 1, 7, @Now, 28800, 1172, N'SchoolCensus2025_Spring', 7400043;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 16, 0, 1, 7, @Now, 27360, 1172, N'SchoolCensus2025_Spring', 7400043;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3942091' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 1, 0, 0, 0, @Now, 2880, 1205, N'SchoolCensus2026_Autumn', 7500106;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 31, 0, 0, 0, @Now, 1560, 1205, N'SchoolCensus2026_Autumn', 7500106;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 11, 9, 6, 0, @Now, 1500, 1205, N'SchoolCensus2026_Autumn', 7500106;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 31, 0, 0, 0, @Now, 240, 1205, N'SchoolCensus2026_Autumn', 7500106;
    EXEC #AddReturnStatus N'East Herrington Primary Academy', N'3942091', 2, 0, 3, 1, @Now, 180, 1205, N'SchoolCensus2026_Autumn', 7500106;
END;

-- Goldington Green Academy (8222008)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8222008' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Goldington Green Academy', N'8222008', 7, 0, 1, 22, @Now, 12199, 1172, N'SchoolCensus2025_Spring', 7392680;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8222008' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Goldington Green Academy', N'8222008', 11, 12, 5, 0, @Now, 317, 1205, N'SchoolCensus2026_Autumn', 7500107;
END;

-- Hazelwick School (9384029)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9384029' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hazelwick School', N'9384029', 16, 0, 1, 2, @Now, 12296, 1172, N'SchoolCensus2025_Spring', 7403216;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9384029' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Hazelwick School', N'9384029', 14, 0, 2, 4, @Now, 358, 1205, N'SchoolCensus2026_Autumn', 7500108;
END;

-- Heartlands Academy (3306905)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3306905' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Heartlands Academy', N'3306905', 16, 0, 1, 4, @Now, 12393, 1172, N'SchoolCensus2025_Spring', 7400881;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3306905' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Heartlands Academy', N'3306905', 4, 0, 1, 6, @Now, 399, 1205, N'SchoolCensus2026_Autumn', 7500109;
END;

-- Heath Lane Nursery School (9191009)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9191009' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Heath Lane Nursery School', N'9191009', 16, 0, 1, 35, @Now, 12490, 1172, N'SchoolCensus2025_Spring', 7400435;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9191009' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Heath Lane Nursery School', N'9191009', 15, 0, 1, 3, @Now, 440, 1205, N'SchoolCensus2026_Autumn', 7500110;
END;

-- Manor Park Primary School (3312141)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3312141' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Manor Park Primary School', N'3312141', 7, 0, 1, 13, @Now, 12587, 1172, N'SchoolCensus2025_Spring', 7387561;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3312141' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Manor Park Primary School', N'3312141', 7, 0, 1, 5, @Now, 481, 1205, N'SchoolCensus2026_Autumn', 7500111;
END;

-- Muriel Green Nursery School (9191028)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9191028' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Muriel Green Nursery School', N'9191028', 7, 0, 1, 3, @Now, 12684, 1172, N'SchoolCensus2025_Spring', 7390085;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9191028' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Muriel Green Nursery School', N'9191028', 5, 0, 2, 4, @Now, 522, 1205, N'SchoolCensus2026_Autumn', 7500112;
END;

-- Oakridge Junior School (8502227)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8502227' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Oakridge Junior School', N'8502227', 7, 0, 1, 1, @Now, 12781, 1172, N'SchoolCensus2025_Spring', 7391670;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8502227' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Oakridge Junior School', N'8502227', 16, 0, 0, 2, @Now, 563, 1205, N'SchoolCensus2026_Autumn', 7500113;
END;

-- Park Spring Primary School (3832488)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3832488' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Park Spring Primary School', N'3832488', 7, 0, 1, 35, @Now, 12878, 1172, N'SchoolCensus2025_Spring', 7388206;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3832488' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Park Spring Primary School', N'3832488', 10, 0, 1, 9, @Now, 604, 1205, N'SchoolCensus2026_Autumn', 7500114;
END;
