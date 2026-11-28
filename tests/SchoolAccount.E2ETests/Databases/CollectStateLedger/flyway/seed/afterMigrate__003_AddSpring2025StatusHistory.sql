-- A school that already has rows for the census is left alone, so re-running migrate adds nothing.
-- Generated from a sample of the CollectStateLedger rows that AddChangedCollectReturnStatus wrote
-- from a restored COLLECTPortal backup. School names and LAEStabs are public (GIAS) data.
-- UpdatedAt is relative to when the seed runs, so the data always looks recent.

DECLARE @Now DATETIME = GETUTCDATE();

-- Bradley Stoke Community School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8034104' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 1, 0, 0, 0, @Now, 30240, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 31, 0, 0, 0, @Now, 28800, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 2, 4, 9, 0, @Now, 28680, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 2, 0, 3, 6, @Now, 25920, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 4, 0, 3, 6, @Now, 24480, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 7, 0, 3, 6, @Now, 1200, 1172, N'SchoolCensus2025_Spring', 7387383;
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 10, 0, 3, 6, @Now, 180, 1172, N'SchoolCensus2025_Spring', 7387383;
END;

-- Barnabas Oley CofE Primary School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8733067' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 1, 0, 0, 0, @Now, 36007, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 31, 0, 0, 0, @Now, 34567, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 11, 17, 4, 0, @Now, 34507, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 31, 0, 0, 0, @Now, 33127, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 2, 2, 6, 0, @Now, 33067, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 4, 0, 6, 2, @Now, 31687, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 5, 0, 6, 2, @Now, 30247, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 3, 0, 2, 6, @Now, 28807, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 4, 0, 1, 7, @Now, 27367, 1172, N'SchoolCensus2025_Spring', 7402139;
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 7, 0, 1, 7, @Now, 307, 1172, N'SchoolCensus2025_Spring', 7402139;
END;

-- Walsgrave Church of England Academy
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3313006' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 1, 0, 0, 0, @Now, 43214, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 31, 0, 0, 0, @Now, 41774, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 2, 0, 5, 0, @Now, 41714, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 4, 0, 1, 4, @Now, 40334, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 7, 0, 1, 4, @Now, 37454, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 9, 0, 1, 5, @Now, 5774, 1172, N'SchoolCensus2025_Spring', 7395579;
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 7, 0, 0, 5, @Now, 614, 1172, N'SchoolCensus2025_Spring', 7395579;
END;

-- The Pinetree School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9261112' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 1, 0, 0, 0, @Now, 40341, 1172, N'SchoolCensus2025_Spring', 7404688;
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 31, 0, 0, 0, @Now, 38901, 1172, N'SchoolCensus2025_Spring', 7404688;
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 2, 1, 2, 0, @Now, 38841, 1172, N'SchoolCensus2025_Spring', 7404688;
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 4, 0, 1, 3, @Now, 37461, 1172, N'SchoolCensus2025_Spring', 7404688;
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 16, 0, 1, 3, @Now, 36021, 1172, N'SchoolCensus2025_Spring', 7404688;
END;

-- St Andrew's Church of England (VA) Infant School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3813315' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 1, 0, 0, 0, @Now, 37468, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 31, 0, 0, 0, @Now, 37228, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 2, 0, 8, 0, @Now, 37168, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 4, 0, 2, 6, @Now, 36028, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 8, 0, 2, 6, @Now, 21628, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 3, 0, 1, 7, @Now, 20188, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 4, 0, 0, 8, @Now, 18748, 1172, N'SchoolCensus2025_Spring', 7395469;
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 7, 0, 0, 8, @Now, 17308, 1172, N'SchoolCensus2025_Spring', 7395469;
END;

-- Thornsett Primary School (High Peak Federation)
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8302182' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 1, 0, 0, 0, @Now, 31715, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 32, 0, 0, 0, @Now, 30275, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 31, 0, 0, 0, @Now, 30095, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 2, 0, 4, 0, @Now, 30035, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 4, 0, 1, 3, @Now, 28835, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 15, 0, 1, 3, @Now, 27395, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 7, 0, 1, 3, @Now, 25955, 1172, N'SchoolCensus2025_Spring', 7393489;
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 10, 0, 1, 3, @Now, 2915, 1172, N'SchoolCensus2025_Spring', 7393489;
END;

-- Queen's Church of England Academy
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9372025' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 1, 0, 0, 0, @Now, 38922, 1172, N'SchoolCensus2025_Spring', 7399251;
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 31, 0, 0, 0, @Now, 37482, 1172, N'SchoolCensus2025_Spring', 7399251;
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 2, 0, 3, 0, @Now, 37422, 1172, N'SchoolCensus2025_Spring', 7399251;
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 4, 0, 1, 2, @Now, 36042, 1172, N'SchoolCensus2025_Spring', 7399251;
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 7, 0, 1, 2, @Now, 34602, 1172, N'SchoolCensus2025_Spring', 7399251;
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 9, 0, 2, 2, @Now, 402, 1172, N'SchoolCensus2025_Spring', 7399251;
END;

-- Valley Invicta Primary School At Aylesford
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8862030' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 1, 0, 0, 0, @Now, 30289, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 31, 0, 0, 0, @Now, 28849, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 2, 4, 9, 0, @Now, 28729, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 2, 0, 3, 6, @Now, 25969, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 4, 0, 3, 6, @Now, 24529, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 7, 0, 3, 6, @Now, 1249, 1172, N'SchoolCensus2025_Spring', 7404731;
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 10, 0, 3, 6, @Now, 229, 1172, N'SchoolCensus2025_Spring', 7404731;
END;

-- Hill View Junior Academy
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3942038' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 1, 0, 0, 0, @Now, 36056, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 31, 0, 0, 0, @Now, 34616, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 11, 17, 4, 0, @Now, 34556, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 31, 0, 0, 0, @Now, 33176, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 2, 2, 6, 0, @Now, 33116, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 4, 0, 6, 2, @Now, 31736, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 5, 0, 6, 2, @Now, 30296, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 3, 0, 2, 6, @Now, 28856, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 4, 0, 1, 7, @Now, 27416, 1172, N'SchoolCensus2025_Spring', 7383636;
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 7, 0, 1, 7, @Now, 356, 1172, N'SchoolCensus2025_Spring', 7383636;
END;

-- St Faiths CofE Primary School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9263120' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 1, 0, 0, 0, @Now, 43263, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 31, 0, 0, 0, @Now, 41823, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 2, 0, 5, 0, @Now, 41763, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 4, 0, 1, 4, @Now, 40383, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 7, 0, 1, 4, @Now, 37503, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 9, 0, 1, 5, @Now, 5823, 1172, N'SchoolCensus2025_Spring', 7399370;
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 7, 0, 0, 5, @Now, 663, 1172, N'SchoolCensus2025_Spring', 7399370;
END;

-- St Faith's CofE Primary School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'2123434' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 1, 0, 0, 0, @Now, 40390, 1172, N'SchoolCensus2025_Spring', 7397122;
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 31, 0, 0, 0, @Now, 38950, 1172, N'SchoolCensus2025_Spring', 7397122;
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 2, 1, 2, 0, @Now, 38890, 1172, N'SchoolCensus2025_Spring', 7397122;
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 4, 0, 1, 3, @Now, 37510, 1172, N'SchoolCensus2025_Spring', 7397122;
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 16, 0, 1, 3, @Now, 36070, 1172, N'SchoolCensus2025_Spring', 7397122;
END;

-- Salisbury, Manor Fields Primary School
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8652191' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 1, 0, 0, 0, @Now, 37517, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 31, 0, 0, 0, @Now, 37277, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 2, 0, 8, 0, @Now, 37217, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 4, 0, 2, 6, @Now, 36077, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 8, 0, 2, 6, @Now, 21677, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 3, 0, 1, 7, @Now, 20237, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 4, 0, 0, 8, @Now, 18797, 1172, N'SchoolCensus2025_Spring', 7401291;
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 7, 0, 0, 8, @Now, 17357, 1172, N'SchoolCensus2025_Spring', 7401291;
END;

-- Goldenhill Primary Academy
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8612009' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 1, 0, 0, 0, @Now, 33204, 1172, N'SchoolCensus2025_Spring', 7383946;
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 31, 0, 0, 0, @Now, 31764, 1172, N'SchoolCensus2025_Spring', 7383946;
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 2, 2, 3, 0, @Now, 31704, 1172, N'SchoolCensus2025_Spring', 7383946;
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 2, 0, 1, 4, @Now, 30324, 1172, N'SchoolCensus2025_Spring', 7383946;
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 4, 0, 1, 5, @Now, 28884, 1172, N'SchoolCensus2025_Spring', 7383946;
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 7, 0, 1, 5, @Now, 27444, 1172, N'SchoolCensus2025_Spring', 7383946;
END;
