-- A school that already has rows for the census is left alone, so re-running migrate adds nothing.
-- Generated from a sample of the CollectStateLedger rows that AddChangedCollectReturnStatus wrote
-- from a restored COLLECTPortal backup. School names and LAEStabs are public (GIAS) data.
-- UpdatedAt is relative to when the seed runs, so the data always looks recent.

DECLARE @Now DATETIME = GETUTCDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9414018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Northampton International Academy', N'9414018', 16, 1, 1, 6, @Now, 10080, 1172, N'SchoolCensus2025_Spring', 7401138;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Christopher''s Church of England School, Cowley', N'9312010', 16, 1, 1, 4, @Now, 10177, 1172, N'SchoolCensus2025_Spring', 7403196;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8884018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Colne Park High School', N'8884018', 16, 1, 1, 3, @Now, 10274, 1172, N'SchoolCensus2025_Spring', 7401492;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8881112' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Coal Clough Academy', N'8881112', 9, 1, 1, 27, @Now, 10371, 1172, N'SchoolCensus2025_Spring', 7395080;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9354017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hadleigh High School', N'9354017', 7, 1, 1, 16, @Now, 10468, 1172, N'SchoolCensus2025_Spring', 7391430;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9354016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Set Beccles School', N'9354016', 16, 1, 0, 6, @Now, 10565, 1172, N'SchoolCensus2025_Spring', 7404516;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9363915' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Michael Catholic Primary School & Nursery', N'9363915', 7, 1, 1, 5, @Now, 10662, 1172, N'SchoolCensus2025_Spring', 7385926;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9352212' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Ixworth Church of England Primary School', N'9352212', 7, 1, 0, 16, @Now, 10759, 1172, N'SchoolCensus2025_Spring', 7384499;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Meadowside Community Primary and Nursery School', N'8772016', 7, 0, 1, 9, @Now, 10856, 1172, N'SchoolCensus2025_Spring', 7390829;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Lingdale Primary School', N'8072016', 7, 0, 1, 8, @Now, 10953, 1172, N'SchoolCensus2025_Spring', 7386294;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Huntcliff School', N'8074015', 7, 0, 0, 14, @Now, 11050, 1172, N'SchoolCensus2025_Spring', 7385149;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Rye Hills Academy', N'8074016', 7, 0, 1, 6, @Now, 11147, 1172, N'SchoolCensus2025_Spring', 7385511;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Outwood Academy Normanby', N'8074013', 7, 0, 1, 27, @Now, 11244, 1172, N'SchoolCensus2025_Spring', 7390373;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Freebrough Academy', N'8074012', 7, 0, 1, 6, @Now, 11341, 1172, N'SchoolCensus2025_Spring', 7390372;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8673062' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Winkfield St Mary''s CofE Primary School', N'8673062', 7, 0, 1, 5, @Now, 11438, 1172, N'SchoolCensus2025_Spring', 7388864;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8674061' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'King''s Academy Easthampstead Park', N'8674061', 7, 0, 1, 7, @Now, 11535, 1172, N'SchoolCensus2025_Spring', 7385962;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672065' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Wildmoor Heath School', N'8672065', 7, 0, 1, 4, @Now, 11632, 1172, N'SchoolCensus2025_Spring', 7384552;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Northwood Primary School', N'9212016', 7, 0, 0, 5, @Now, 11729, 1172, N'SchoolCensus2025_Spring', 7388954;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'John Watson School', N'9317011', 7, 0, 0, 2, @Now, 11826, 1172, N'SchoolCensus2025_Spring', 7383843;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412067' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Long Buckby Junior School', N'9412067', 7, 0, 1, 2, @Now, 11923, 1172, N'SchoolCensus2025_Spring', 7385327;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Springfield School', N'9317012', 7, 0, 0, 3, @Now, 12020, 1172, N'SchoolCensus2025_Spring', 7389586;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Frank Wise School', N'9317010', 7, 0, 1, 4, @Now, 12117, 1172, N'SchoolCensus2025_Spring', 7383842;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412069' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Maidwell Primary School', N'9412069', 7, 0, 1, 3, @Now, 12214, 1172, N'SchoolCensus2025_Spring', 7385329;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412068' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Long Buckby Infant School', N'9412068', 7, 0, 1, 4, @Now, 12311, 1172, N'SchoolCensus2025_Spring', 7385328;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413062' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Syresham St James CofE Primary School and Nursery', N'9413062', 7, 0, 1, 3, @Now, 12408, 1172, N'SchoolCensus2025_Spring', 7385359;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Braunston Church of England Primary School', N'9413011', 7, 0, 1, 5, @Now, 12505, 1172, N'SchoolCensus2025_Spring', 7391044;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Brixworth CofE VC Primary School', N'9413012', 7, 0, 1, 8, @Now, 12602, 1172, N'SchoolCensus2025_Spring', 7385351;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413060' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Stoke Bruerne Church of England Primary School', N'9413060', 7, 0, 1, 2, @Now, 12699, 1172, N'SchoolCensus2025_Spring', 7385358;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Iffley Academy', N'9317018', 7, 0, 1, 5, @Now, 12796, 1172, N'SchoolCensus2025_Spring', 7392629;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bridgewater Primary School', N'9412010', 7, 0, 1, 3, @Now, 12893, 1172, N'SchoolCensus2025_Spring', 7385317;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9411010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Camrose Early Years Centre for Children & Families', N'9411010', 7, 0, 1, 18, @Now, 12990, 1172, N'SchoolCensus2025_Spring', 7385312;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Byfield School', N'9412011', 7, 0, 1, 6, @Now, 13087, 1172, N'SchoolCensus2025_Spring', 7383573;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Bramptons Primary School', N'9412012', 7, 0, 1, 2, @Now, 13184, 1172, N'SchoolCensus2025_Spring', 7385318;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412064' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Sunnyside Primary Academy', N'9412064', 7, 0, 1, 5, @Now, 13281, 1172, N'SchoolCensus2025_Spring', 7386763;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Chipping Warden Primary Academy', N'9412013', 7, 0, 0, 4, @Now, 13378, 1172, N'SchoolCensus2025_Spring', 7389547;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Croughton All Saints CofE Primary School', N'9413019', 7, 0, 1, 6, @Now, 13475, 1172, N'SchoolCensus2025_Spring', 7385352;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Delapre Primary School', N'9412014', 7, 0, 1, 3, @Now, 13572, 1172, N'SchoolCensus2025_Spring', 7385319;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412063' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Woodvale Primary Academy', N'9412063', 7, 0, 0, 10, @Now, 13669, 1172, N'SchoolCensus2025_Spring', 7386762;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cogenhoe Primary School', N'9412015', 7, 0, 1, 3, @Now, 13766, 1172, N'SchoolCensus2025_Spring', 7385785;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Millway Primary School', N'9412016', 7, 0, 1, 2, @Now, 13863, 1172, N'SchoolCensus2025_Spring', 7385320;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412065' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Kings Sutton Primary Academy', N'9412065', 7, 0, 1, 4, @Now, 13960, 1172, N'SchoolCensus2025_Spring', 7389548;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Oxford Academy', N'9314019', 7, 0, 1, 3, @Now, 14057, 1172, N'SchoolCensus2025_Spring', 7385132;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Millbrook Primary School', N'9312016', 7, 0, 3, 3, @Now, 14154, 1172, N'SchoolCensus2025_Spring', 7392347;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Heyford Park School', N'9314018', 7, 0, 1, 5, @Now, 14251, 1172, N'SchoolCensus2025_Spring', 7385131;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Eynsham Community Primary School', N'9312013', 7, 0, 0, 3, @Now, 14348, 1172, N'SchoolCensus2025_Spring', 7388319;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Whitelands Academy', N'9314017', 7, 0, 0, 2, @Now, 14445, 1172, N'SchoolCensus2025_Spring', 7390004;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9417017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Northgate School Arts College', N'9417017', 7, 0, 1, 3, @Now, 14542, 1172, N'SchoolCensus2025_Spring', 7389887;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9417018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Gateway School', N'9417018', 7, 0, 1, 5, @Now, 14639, 1172, N'SchoolCensus2025_Spring', 7386456;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9417019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Greenfields Specialist School for Communication', N'9417019', 7, 0, 1, 5, @Now, 14736, 1172, N'SchoolCensus2025_Spring', 7393158;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9417014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Fairfields School', N'9417014', 7, 0, 1, 5, @Now, 14833, 1172, N'SchoolCensus2025_Spring', 7385373;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Bicester School', N'9314011', 7, 0, 1, 1, @Now, 14930, 1172, N'SchoolCensus2025_Spring', 7393274;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Swan School', N'9314016', 7, 0, 1, 2, @Now, 15027, 1172, N'SchoolCensus2025_Spring', 7390362;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313064' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bloxham Church of England Primary School', N'9313064', 7, 0, 0, 1, @Now, 15124, 1172, N'SchoolCensus2025_Spring', 7391636;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Greyfriars Catholic School', N'9314014', 7, 0, 1, 6, @Now, 15221, 1172, N'SchoolCensus2025_Spring', 7385134;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313065' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Fritwell Church of England Primary School', N'9313065', 7, 0, 1, 5, @Now, 15318, 1172, N'SchoolCensus2025_Spring', 7386286;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Longford Park Primary School', N'9312017', 7, 0, 0, 7, @Now, 15415, 1172, N'SchoolCensus2025_Spring', 7384422;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422717' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Patrick''s Catholic Primary School', N'9422717', 7, 0, 1, 4, @Now, 15512, 1172, N'SchoolCensus2025_Spring', 7387004;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422718' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Distington Community School', N'9422718', 7, 0, 1, 2, @Now, 15609, 1172, N'SchoolCensus2025_Spring', 7386926;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422710' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Robert Ferguson Primary School', N'9422710', 7, 0, 1, 2, @Now, 15706, 1172, N'SchoolCensus2025_Spring', 7386974;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422711' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Upperby Primary School', N'9422711', 7, 0, 1, 2, @Now, 15803, 1172, N'SchoolCensus2025_Spring', 7387011;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422713' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Pennine Way Primary School', N'9422713', 7, 0, 1, 3, @Now, 15900, 1172, N'SchoolCensus2025_Spring', 7386970;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422714' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Orgill Primary School', N'9422714', 7, 0, 1, 5, @Now, 15997, 1172, N'SchoolCensus2025_Spring', 7386967;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422716' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Montreal CofE Primary School', N'9422716', 7, 0, 1, 0, @Now, 16094, 1172, N'SchoolCensus2025_Spring', 7386960;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772112' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Stockton Heath Primary School', N'8772112', 7, 0, 1, 3, @Now, 16191, 1172, N'SchoolCensus2025_Spring', 7384267;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772116' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Thelwall Community Junior School', N'8772116', 7, 0, 1, 2, @Now, 16288, 1172, N'SchoolCensus2025_Spring', 7384268;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772117' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Cobbs Infant and Nursery School', N'8772117', 7, 0, 1, 18, @Now, 16385, 1172, N'SchoolCensus2025_Spring', 7384269;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072166' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Nunthorpe Primary Academy', N'8072166', 7, 0, 1, 12, @Now, 16482, 1172, N'SchoolCensus2025_Spring', 7391064;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672165' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Birch Hill Primary School', N'8672165', 7, 0, 1, 4, @Now, 16579, 1172, N'SchoolCensus2025_Spring', 7386248;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573114' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Langham CofE (Controlled) Primary School', N'8573114', 7, 0, 1, 10, @Now, 16676, 1172, N'SchoolCensus2025_Spring', 7387837;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573115' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Oakham CofE Primary School', N'8573115', 7, 0, 1, 8, @Now, 16773, 1172, N'SchoolCensus2025_Spring', 7386059;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573112' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Exton and Greetham CofE Primary School', N'8573112', 7, 0, 1, 10, @Now, 16870, 1172, N'SchoolCensus2025_Spring', 7384640;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412118' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St James Infant School', N'9412118', 7, 0, 0, 9, @Now, 16967, 1172, N'SchoolCensus2025_Spring', 7384621;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412160' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Earl Spencer Primary School', N'9412160', 7, 0, 1, 7, @Now, 17064, 1172, N'SchoolCensus2025_Spring', 7385337;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412162' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Headlands Primary School', N'9412162', 7, 0, 1, 11, @Now, 17161, 1172, N'SchoolCensus2025_Spring', 7392033;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412161' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Falconer''s Hill Academy', N'9412161', 7, 0, 0, 2, @Now, 17258, 1172, N'SchoolCensus2025_Spring', 7388331;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412164' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Malcolm Arnold Preparatory School', N'9412164', 7, 0, 1, 5, @Now, 17355, 1172, N'SchoolCensus2025_Spring', 7387094;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313167' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Garsington Church of England Primary School', N'9313167', 7, 0, 1, 5, @Now, 17452, 1172, N'SchoolCensus2025_Spring', 7383894;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313161' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Horspath Church of England Primary School', N'9313161', 7, 0, 1, 3, @Now, 17549, 1172, N'SchoolCensus2025_Spring', 7383893;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9162068' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Eastington Primary School', N'9162068', 7, 0, 1, 2, @Now, 17646, 1172, N'SchoolCensus2025_Spring', 7390540;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9163061' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Field Court Church of England Infant Academy', N'9163061', 7, 0, 1, 6, @Now, 17743, 1172, N'SchoolCensus2025_Spring', 7392688;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9163011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hempsted Church of England Primary School', N'9163011', 7, 0, 1, 1, @Now, 17840, 1172, N'SchoolCensus2025_Spring', 7390985;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9163010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Kingsholm Church of England Primary School', N'9163010', 7, 0, 1, 0, @Now, 17937, 1172, N'SchoolCensus2025_Spring', 7390984;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9367010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Dominic''s School', N'9367010', 7, 0, 1, 2, @Now, 18034, 1172, N'SchoolCensus2025_Spring', 7384591;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9367065' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Philip Southcote School', N'9367065', 7, 0, 1, 2, @Now, 18131, 1172, N'SchoolCensus2025_Spring', 7386252;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9367067' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Grafham Grange School', N'9367067', 7, 0, 1, 2, @Now, 18228, 1172, N'SchoolCensus2025_Spring', 7384590;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9362013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Woodmansterne Primary School', N'9362013', 7, 0, 1, 5, @Now, 18325, 1172, N'SchoolCensus2025_Spring', 7384817;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9363019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Martins Church of England Junior School', N'9363019', 7, 0, 1, 2, @Now, 18422, 1172, N'SchoolCensus2025_Spring', 7391519;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9362014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Lime Tree Primary School', N'9362014', 7, 0, 1, 3, @Now, 18519, 1172, N'SchoolCensus2025_Spring', 7387872;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9362063' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cranleigh Church of England Primary School', N'9362063', 7, 0, 1, 5, @Now, 18616, 1172, N'SchoolCensus2025_Spring', 7386476;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9364067' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Woolmer Hill School', N'9364067', 7, 0, 1, 3, @Now, 18713, 1172, N'SchoolCensus2025_Spring', 7383737;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8877016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Rivermead School', N'8877016', 9, 0, 0, 6, @Now, 18810, 1172, N'SchoolCensus2025_Spring', 7396376;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Warren Wood Primary Academy', N'8872011', 9, 0, 0, 13, @Now, 18907, 1172, N'SchoolCensus2025_Spring', 7398367;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Napier Community Primary and Nursery Academy', N'8872012', 9, 0, 1, 3, @Now, 19004, 1172, N'SchoolCensus2025_Spring', 7398368;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Temple Mill Primary School', N'8872015', 9, 0, 1, 19, @Now, 19101, 1172, N'SchoolCensus2025_Spring', 7396314;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8874068' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Holcombe Grammar School', N'8874068', 9, 0, 1, 4, @Now, 19198, 1172, N'SchoolCensus2025_Spring', 7397287;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872016' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Byron Primary School', N'8872016', 9, 0, 1, 6, @Now, 19295, 1172, N'SchoolCensus2025_Spring', 7396362;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8874069' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Fort Pitt Grammar School', N'8874069', 9, 0, 0, 1, @Now, 19392, 1172, N'SchoolCensus2025_Spring', 7394272;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cuxton Community Junior School', N'8872013', 9, 0, 1, 2, @Now, 19489, 1172, N'SchoolCensus2025_Spring', 7398369;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Twydall Primary School and Nursery', N'8872014', 9, 0, 0, 4, @Now, 19586, 1172, N'SchoolCensus2025_Spring', 7396405;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cedar Children''s Academy', N'8872017', 9, 0, 1, 6, @Now, 19683, 1172, N'SchoolCensus2025_Spring', 7398250;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8774010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Padgate Academy', N'8774010', 9, 0, 1, 8, @Now, 19780, 1172, N'SchoolCensus2025_Spring', 7393620;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Laurence Jackson School', N'8074014', 9, 0, 0, 4, @Now, 19877, 1172, N'SchoolCensus2025_Spring', 7394436;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Outwood Academy Redcar', N'8074010', 9, 0, 0, 8, @Now, 19974, 1172, N'SchoolCensus2025_Spring', 7395142;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Barton Primary School', N'9212012', 9, 0, 1, 5, @Now, 20071, 1172, N'SchoolCensus2025_Spring', 7396497;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Newchurch Primary School', N'9212010', 9, 0, 1, 2, @Now, 20168, 1172, N'SchoolCensus2025_Spring', 7396496;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Niton Primary School', N'9212015', 9, 0, 0, 2, @Now, 20265, 1172, N'SchoolCensus2025_Spring', 7396499;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Nine Acres Primary School', N'9212014', 9, 0, 0, 3, @Now, 20362, 1172, N'SchoolCensus2025_Spring', 7396498;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hunnyhill Primary School', N'9212018', 9, 0, 0, 1, @Now, 20459, 1172, N'SchoolCensus2025_Spring', 7396500;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9213011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Shalfleet Church of England Primary School', N'9213011', 9, 0, 0, 1, @Now, 20556, 1172, N'SchoolCensus2025_Spring', 7396479;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Grandpont Nursery School', N'9311010', 9, 0, 0, 14, @Now, 20653, 1172, N'SchoolCensus2025_Spring', 7394580;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312012' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Didcot Primary Academy', N'9312012', 9, 0, 1, 2, @Now, 20750, 1172, N'SchoolCensus2025_Spring', 7398526;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Slade Nursery School', N'9311011', 9, 0, 0, 28, @Now, 20847, 1172, N'SchoolCensus2025_Spring', 7394581;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312060' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hardwick Primary School', N'9312060', 9, 0, 0, 4, @Now, 20944, 1172, N'SchoolCensus2025_Spring', 7395373;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Chipping Norton School', N'9314010', 9, 0, 1, 4, @Now, 21041, 1172, N'SchoolCensus2025_Spring', 7396803;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311017' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Lydalls Nursery School', N'9311017', 9, 0, 0, 2, @Now, 21138, 1172, N'SchoolCensus2025_Spring', 7394582;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'William Morris Primary School', N'9312019', 9, 0, 0, 6, @Now, 21235, 1172, N'SchoolCensus2025_Spring', 7396407;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The ACE Centre Nursery School', N'9311019', 9, 0, 0, 5, @Now, 21332, 1172, N'SchoolCensus2025_Spring', 7394583;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Maiden Erlegh Chiltern Edge', N'9314013', 9, 0, 0, 3, @Now, 21429, 1172, N'SchoolCensus2025_Spring', 7396675;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573111' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Empingham Church of England Primary School', N'8573111', 9, 0, 1, 5, @Now, 21526, 1172, N'SchoolCensus2025_Spring', 7395299;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573119' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Uppingham Church of England Primary School', N'8573119', 9, 0, 1, 15, @Now, 21623, 1172, N'SchoolCensus2025_Spring', 7395301;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412169' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Abbey Primary School', N'9412169', 9, 0, 1, 6, @Now, 21720, 1172, N'SchoolCensus2025_Spring', 7395329;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412166' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Kingsley Primary School', N'9412166', 9, 0, 1, 4, @Now, 21817, 1172, N'SchoolCensus2025_Spring', 7395332;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872211' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Halling Primary School', N'8872211', 9, 0, 1, 4, @Now, 21914, 1172, N'SchoolCensus2025_Spring', 7395425;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072218' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Green Gates Primary School', N'8072218', 9, 0, 1, 15, @Now, 22011, 1172, N'SchoolCensus2025_Spring', 7398018;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312210' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'King''s Meadow Primary School', N'9312210', 9, 0, 0, 4, @Now, 22108, 1172, N'SchoolCensus2025_Spring', 7395556;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Lockwood Primary School', N'8072018', 10, 0, 1, 5, @Now, 22205, 1172, N'SchoolCensus2025_Spring', 7399551;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672060' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Holly Spring Primary School', N'8672060', 10, 0, 0, 2, @Now, 22302, 1172, N'SchoolCensus2025_Spring', 7399160;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672813' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Whitegrove Primary School', N'8672813', 10, 0, 0, 2, @Now, 22399, 1172, N'SchoolCensus2025_Spring', 7399162;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9425413' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Caldew School', N'9425413', 10, 0, 0, 6, @Now, 22496, 1172, N'SchoolCensus2025_Spring', 7399610;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8892169' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cedars Primary School', N'8892169', 10, 0, 0, 3, @Now, 22593, 1172, N'SchoolCensus2025_Spring', 7398910;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8892168' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Shadsworth Junior School', N'8892168', 10, 0, 1, 1, @Now, 22690, 1172, N'SchoolCensus2025_Spring', 7398909;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8892167' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Shadsworth Infant School', N'8892167', 10, 0, 1, 12, @Now, 22787, 1172, N'SchoolCensus2025_Spring', 7398908;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8884718' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Cardinal Allen Catholic High School, Fleetwood', N'8884718', 10, 0, 1, 3, @Now, 22884, 1172, N'SchoolCensus2025_Spring', 7399515;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872010' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Gordons Children''s Academy, Infant', N'8872010', 16, 0, 0, 5, @Now, 22981, 1172, N'SchoolCensus2025_Spring', 7403185;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872018' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Wayfield Primary School', N'8872018', 16, 0, 1, 4, @Now, 23078, 1172, N'SchoolCensus2025_Spring', 7405000;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872019' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Featherby Junior School', N'8872019', 16, 0, 1, 5, @Now, 23175, 1172, N'SchoolCensus2025_Spring', 7402680;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772013' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bewsey Lodge Primary School', N'8772013', 16, 0, 1, 5, @Now, 23272, 1172, N'SchoolCensus2025_Spring', 7404292;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Dallam Community Primary School', N'8772015', 16, 0, 1, 11, @Now, 23369, 1172, N'SchoolCensus2025_Spring', 7405234;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9414066' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Duston School', N'9414066', 16, 0, 0, 6, @Now, 23466, 1172, N'SchoolCensus2025_Spring', 7403578;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9414011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Silverstone UTC', N'9414011', 16, 0, 0, 6, @Now, 23563, 1172, N'SchoolCensus2025_Spring', 7402423;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312015' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Bayards Hill School', N'9312015', 16, 0, 1, 5, @Now, 23660, 1172, N'SchoolCensus2025_Spring', 7400505;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312014' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Gagle Brook Primary School', N'9312014', 16, 0, 1, 11, @Now, 23757, 1172, N'SchoolCensus2025_Spring', 7401534;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312011' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Aureus Primary School', N'9312011', 16, 0, 0, 2, @Now, 23854, 1172, N'SchoolCensus2025_Spring', 7401533;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314060' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Gosford Hill School', N'9314060', 16, 0, 1, 1, @Now, 23951, 1172, N'SchoolCensus2025_Spring', 7400275;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672113' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Uplands Primary School and Nursery', N'8672113', 16, 0, 0, 10, @Now, 24048, 1172, N'SchoolCensus2025_Spring', 7405391;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573113' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Ketton Church of England Primary School', N'8573113', 16, 0, 0, 6, @Now, 24145, 1172, N'SchoolCensus2025_Spring', 7400483;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573117' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Whissendine Church of England Primary School', N'8573117', 16, 0, 1, 7, @Now, 24242, 1172, N'SchoolCensus2025_Spring', 7400484;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412117' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Waynflete Infants'' School', N'9412117', 16, 0, 0, 6, @Now, 24339, 1172, N'SchoolCensus2025_Spring', 7404584;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314116' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Cherwell School', N'9314116', 16, 0, 1, 3, @Now, 24436, 1172, N'SchoolCensus2025_Spring', 7404590;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872213' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hoo St Werburgh Primary School and the Marlborough', N'8872213', 16, 0, 1, 5, @Now, 24533, 1172, N'SchoolCensus2025_Spring', 7401885;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872214' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Balfour Junior School', N'8872214', 16, 0, 1, 1, @Now, 24630, 1172, N'SchoolCensus2025_Spring', 7403482;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9415212' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Welford Sibbertoft and Sulby Endowed School', N'9415212', 16, 0, 1, 3, @Now, 24727, 1172, N'SchoolCensus2025_Spring', 7402391;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072315' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Badger Hill Academy', N'8072315', 16, 0, 1, 6, @Now, 24824, 1172, N'SchoolCensus2025_Spring', 7404180;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8572313' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Catmose Primary', N'8572313', 16, 0, 1, 10, @Now, 24921, 1172, N'SchoolCensus2025_Spring', 7403326;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312465' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Mill Lane Community Primary School', N'9312465', 16, 0, 1, 2, @Now, 25018, 1172, N'SchoolCensus2025_Spring', 7404889;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312463' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Barley Hill Primary School', N'9312463', 16, 0, 0, 0, @Now, 25115, 1172, N'SchoolCensus2025_Spring', 7404888;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772416' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Newchurch Community Primary School', N'8772416', 16, 0, 1, 2, @Now, 25212, 1172, N'SchoolCensus2025_Spring', 7400196;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312565' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Dry Sandford Primary School', N'9312565', 16, 0, 0, 3, @Now, 25309, 1172, N'SchoolCensus2025_Spring', 7404894;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312563' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Harwell Primary School', N'9312563', 16, 0, 1, 1, @Now, 25406, 1172, N'SchoolCensus2025_Spring', 7404893;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312562' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Elms Primary School', N'9312562', 16, 0, 1, 4, @Now, 25503, 1172, N'SchoolCensus2025_Spring', 7404594;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312561' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Folly View Primary', N'9312561', 16, 0, 1, 11, @Now, 25600, 1172, N'SchoolCensus2025_Spring', 7404592;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312560' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Drayton Community Primary School', N'9312560', 16, 0, 0, 3, @Now, 25697, 1172, N'SchoolCensus2025_Spring', 7404578;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314560' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'The Marlborough Church of England School', N'9314560', 16, 0, 0, 0, @Now, 25794, 1172, N'SchoolCensus2025_Spring', 7402211;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872665' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Peter''s Infant School', N'8872665', 16, 0, 0, 3, @Now, 25891, 1172, N'SchoolCensus2025_Spring', 7404111;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8873712' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Michael''s RC Primary School', N'8873712', 16, 0, 0, 3, @Now, 25988, 1172, N'SchoolCensus2025_Spring', 7401300;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9425216' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Gilsland CofE Primary School', N'9425216', 16, 0, 1, 9, @Now, 26085, 1172, N'SchoolCensus2025_Spring', 7403545;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9425219' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Flimby Primary School', N'9425219', 16, 0, 1, 5, @Now, 26182, 1172, N'SchoolCensus2025_Spring', 7402359;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313810' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Shiplake Church of England School', N'9313810', 16, 0, 0, 4, @Now, 26279, 1172, N'SchoolCensus2025_Spring', 7400020;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313861' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Dunmore Primary School', N'9313861', 16, 0, 1, 4, @Now, 26376, 1172, N'SchoolCensus2025_Spring', 7404124;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9416910' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Malcolm Arnold Academy', N'9416910', 16, 0, 0, 4, @Now, 26473, 1172, N'SchoolCensus2025_Spring', 7402445;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9425412' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'William Howard School', N'9425412', 16, 0, 1, 2, @Now, 26570, 1172, N'SchoolCensus2025_Spring', 7399614;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912865' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Robert Miles Junior School', N'8912865', 16, 0, 1, 15, @Now, 26667, 1172, N'SchoolCensus2025_Spring', 7401899;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8222115' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Oakley Primary Academy', N'8222115', 16, 0, 1, 4, @Now, 26764, 1172, N'SchoolCensus2025_Spring', 7402831;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8023117' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Christ Church Church of England Primary School', N'8023117', 16, 0, 1, 10, @Now, 26861, 1172, N'SchoolCensus2025_Spring', 7402662;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8023118' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Martin''s Church of England Primary School', N'8023118', 16, 0, 1, 6, @Now, 26958, 1172, N'SchoolCensus2025_Spring', 7402663;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8023116' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Hutton Church of England Primary School', N'8023116', 16, 0, 1, 9, @Now, 27055, 1172, N'SchoolCensus2025_Spring', 7402661;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912913' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Morven Park Primary and Nursery School', N'8912913', 16, 0, 1, 36, @Now, 27152, 1172, N'SchoolCensus2025_Spring', 7400616;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912916' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Arnold Mill Primary and Nursery School', N'8912916', 16, 0, 1, 11, @Now, 27249, 1172, N'SchoolCensus2025_Spring', 7400617;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912918' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Orchard Primary School and Nursery', N'8912918', 16, 0, 1, 14, @Now, 27346, 1172, N'SchoolCensus2025_Spring', 7400618;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912919' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Greenwood Primary and Nursery School', N'8912919', 16, 0, 1, 7, @Now, 27443, 1172, N'SchoolCensus2025_Spring', 7401901;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8812915' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Westwood Academy', N'8812915', 16, 0, 1, 5, @Now, 27540, 1172, N'SchoolCensus2025_Spring', 7401789;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8816911' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'Colchester Academy', N'8816911', 16, 0, 0, 7, @Now, 27637, 1172, N'SchoolCensus2025_Spring', 7403716;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8525417' AND Collection = N'SchoolCensus2025_Spring')
BEGIN
    EXEC #AddReturnStatus N'St Anne''s Catholic School', N'8525417', 16, 0, 1, 0, @Now, 27734, 1172, N'SchoolCensus2025_Spring', 7400699;
END;
