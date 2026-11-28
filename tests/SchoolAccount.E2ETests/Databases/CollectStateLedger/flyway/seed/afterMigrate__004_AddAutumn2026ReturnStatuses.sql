-- A school that already has rows for the census is left alone, so re-running migrate adds nothing.
-- DCID 1205 and the DataReturnIDs are invented: COLLECTPortal has no SchoolCensus2026_Autumn yet.
-- Generated from a sample of the CollectStateLedger rows that AddChangedCollectReturnStatus wrote
-- from a restored COLLECTPortal backup. School names and LAEStabs are public (GIAS) data.
-- UpdatedAt is relative to when the seed runs, so the data always looks recent.

DECLARE @Now DATETIME = GETUTCDATE();

IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8034104' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Bradley Stoke Community School', N'8034104', 1, 0, 0, 0, @Now, 30, 1205, N'SchoolCensus2026_Autumn', 7500000;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8733067' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Barnabas Oley CofE Primary School', N'8733067', 1, 0, 0, 0, @Now, 83, 1205, N'SchoolCensus2026_Autumn', 7500001;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3313006' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Walsgrave Church of England Academy', N'3313006', 13, 0, 0, 0, @Now, 136, 1205, N'SchoolCensus2026_Autumn', 7500002;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9261112' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Pinetree School', N'9261112', 2, 0, 6, 2, @Now, 189, 1205, N'SchoolCensus2026_Autumn', 7500003;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3813315' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'St Andrew''s Church of England (VA) Infant School', N'3813315', 15, 0, 1, 5, @Now, 242, 1205, N'SchoolCensus2026_Autumn', 7500004;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8302182' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Thornsett Primary School (High Peak Federation)', N'8302182', 1, 0, 0, 0, @Now, 295, 1205, N'SchoolCensus2026_Autumn', 7500005;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9372025' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Queen''s Church of England Academy', N'9372025', 32, 0, 0, 0, @Now, 348, 1205, N'SchoolCensus2026_Autumn', 7500006;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8862030' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Valley Invicta Primary School At Aylesford', N'8862030', 2, 3, 11, 0, @Now, 401, 1205, N'SchoolCensus2026_Autumn', 7500007;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'3942038' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Hill View Junior Academy', N'3942038', 4, 0, 0, 3, @Now, 454, 1205, N'SchoolCensus2026_Autumn', 7500008;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9263120' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'St Faiths CofE Primary School', N'9263120', 1, 0, 0, 0, @Now, 507, 1205, N'SchoolCensus2026_Autumn', 7500009;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'2123434' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'St Faith''s CofE Primary School', N'2123434', 31, 0, 0, 0, @Now, 560, 1205, N'SchoolCensus2026_Autumn', 7500010;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8652191' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Salisbury, Manor Fields Primary School', N'8652191', 11, 4, 2, 0, @Now, 613, 1205, N'SchoolCensus2026_Autumn', 7500011;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8612009' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Goldenhill Primary Academy', N'8612009', 2, 0, 3, 1, @Now, 666, 1205, N'SchoolCensus2026_Autumn', 7500012;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9414018' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Northampton International Academy', N'9414018', 1, 0, 0, 0, @Now, 719, 1205, N'SchoolCensus2026_Autumn', 7500013;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8881112' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Coal Clough Academy', N'8881112', 24, 0, 0, 0, @Now, 772, 1205, N'SchoolCensus2026_Autumn', 7500014;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9363915' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'St Michael Catholic Primary School & Nursery', N'9363915', 11, 23, 7, 0, @Now, 825, 1205, N'SchoolCensus2026_Autumn', 7500015;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072016' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Lingdale Primary School', N'8072016', 14, 0, 1, 4, @Now, 878, 1205, N'SchoolCensus2026_Autumn', 7500016;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074013' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Outwood Academy Normanby', N'8074013', 1, 0, 0, 0, @Now, 931, 1205, N'SchoolCensus2026_Autumn', 7500017;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8674061' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'King''s Academy Easthampstead Park', N'8674061', 23, 0, 0, 0, @Now, 984, 1205, N'SchoolCensus2026_Autumn', 7500018;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317011' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'John Watson School', N'9317011', 12, 0, 0, 0, @Now, 1037, 1205, N'SchoolCensus2026_Autumn', 7500019;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9317010' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Frank Wise School', N'9317010', 2, 0, 2, 5, @Now, 1090, 1205, N'SchoolCensus2026_Autumn', 7500020;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413062' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Syresham St James CofE Primary School and Nursery', N'9413062', 1, 0, 0, 0, @Now, 1143, 1205, N'SchoolCensus2026_Autumn', 7500021;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9413060' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Stoke Bruerne Church of England Primary School', N'9413060', 1, 0, 0, 0, @Now, 1196, 1205, N'SchoolCensus2026_Autumn', 7500022;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9411010' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Camrose Early Years Centre for Children & Families', N'9411010', 13, 0, 0, 0, @Now, 1249, 1205, N'SchoolCensus2026_Autumn', 7500023;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412064' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Sunnyside Primary Academy', N'9412064', 2, 0, 6, 2, @Now, 1302, 1205, N'SchoolCensus2026_Autumn', 7500024;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412014' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Delapre Primary School', N'9412014', 15, 0, 1, 5, @Now, 1355, 1205, N'SchoolCensus2026_Autumn', 7500025;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412016' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Millway Primary School', N'9412016', 1, 0, 0, 0, @Now, 1408, 1205, N'SchoolCensus2026_Autumn', 7500026;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312016' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Millbrook Primary School', N'9312016', 32, 0, 0, 0, @Now, 1461, 1205, N'SchoolCensus2026_Autumn', 7500027;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314017' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Whitelands Academy', N'9314017', 2, 3, 11, 0, @Now, 1514, 1205, N'SchoolCensus2026_Autumn', 7500028;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9417019' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Greenfields Specialist School for Communication', N'9417019', 4, 0, 0, 3, @Now, 1567, 1205, N'SchoolCensus2026_Autumn', 7500029;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314016' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Swan School', N'9314016', 1, 0, 0, 0, @Now, 1620, 1205, N'SchoolCensus2026_Autumn', 7500030;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313065' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Fritwell Church of England Primary School', N'9313065', 31, 0, 0, 0, @Now, 1673, 1205, N'SchoolCensus2026_Autumn', 7500031;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422718' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Distington Community School', N'9422718', 11, 4, 2, 0, @Now, 1726, 1205, N'SchoolCensus2026_Autumn', 7500032;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9422713' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Pennine Way Primary School', N'9422713', 4, 0, 1, 6, @Now, 1779, 1205, N'SchoolCensus2026_Autumn', 7500033;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772112' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Stockton Heath Primary School', N'8772112', 1, 0, 0, 0, @Now, 1832, 1205, N'SchoolCensus2026_Autumn', 7500034;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072166' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Nunthorpe Primary Academy', N'8072166', 24, 0, 0, 0, @Now, 1885, 1205, N'SchoolCensus2026_Autumn', 7500035;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8573115' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Oakham CofE Primary School', N'8573115', 11, 23, 7, 0, @Now, 1938, 1205, N'SchoolCensus2026_Autumn', 7500036;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412160' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Earl Spencer Primary School', N'9412160', 14, 0, 1, 4, @Now, 1991, 1205, N'SchoolCensus2026_Autumn', 7500037;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412164' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Malcolm Arnold Preparatory School', N'9412164', 1, 0, 0, 0, @Now, 2044, 1205, N'SchoolCensus2026_Autumn', 7500038;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9162068' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Eastington Primary School', N'9162068', 23, 0, 0, 0, @Now, 2097, 1205, N'SchoolCensus2026_Autumn', 7500039;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9163010' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Kingsholm Church of England Primary School', N'9163010', 12, 0, 0, 0, @Now, 2150, 1205, N'SchoolCensus2026_Autumn', 7500040;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9367067' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Grafham Grange School', N'9367067', 2, 0, 2, 5, @Now, 2203, 1205, N'SchoolCensus2026_Autumn', 7500041;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9362014' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Lime Tree Primary School', N'9362014', 1, 0, 0, 0, @Now, 2256, 1205, N'SchoolCensus2026_Autumn', 7500042;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8877016' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Rivermead School', N'8877016', 1, 0, 0, 0, @Now, 2309, 1205, N'SchoolCensus2026_Autumn', 7500043;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872015' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Temple Mill Primary School', N'8872015', 13, 0, 0, 0, @Now, 2362, 1205, N'SchoolCensus2026_Autumn', 7500044;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8874069' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Fort Pitt Grammar School', N'8874069', 2, 0, 6, 2, @Now, 2415, 1205, N'SchoolCensus2026_Autumn', 7500045;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872017' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Cedar Children''s Academy', N'8872017', 15, 0, 1, 5, @Now, 2468, 1205, N'SchoolCensus2026_Autumn', 7500046;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8074010' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Outwood Academy Redcar', N'8074010', 1, 0, 0, 0, @Now, 2521, 1205, N'SchoolCensus2026_Autumn', 7500047;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9212015' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Niton Primary School', N'9212015', 32, 0, 0, 0, @Now, 2574, 1205, N'SchoolCensus2026_Autumn', 7500048;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9213011' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Shalfleet Church of England Primary School', N'9213011', 2, 3, 11, 0, @Now, 2627, 1205, N'SchoolCensus2026_Autumn', 7500049;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311011' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Slade Nursery School', N'9311011', 4, 0, 0, 3, @Now, 2680, 1205, N'SchoolCensus2026_Autumn', 7500050;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9311017' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Lydalls Nursery School', N'9311017', 1, 0, 0, 0, @Now, 2733, 1205, N'SchoolCensus2026_Autumn', 7500051;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314013' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Maiden Erlegh Chiltern Edge', N'9314013', 31, 0, 0, 0, @Now, 2786, 1205, N'SchoolCensus2026_Autumn', 7500052;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412169' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Abbey Primary School', N'9412169', 11, 4, 2, 0, @Now, 2839, 1205, N'SchoolCensus2026_Autumn', 7500053;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8072218' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Green Gates Primary School', N'8072218', 4, 0, 1, 6, @Now, 2892, 1205, N'SchoolCensus2026_Autumn', 7500054;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672060' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Holly Spring Primary School', N'8672060', 1, 0, 0, 0, @Now, 2945, 1205, N'SchoolCensus2026_Autumn', 7500055;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8892169' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Cedars Primary School', N'8892169', 24, 0, 0, 0, @Now, 2998, 1205, N'SchoolCensus2026_Autumn', 7500056;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8884718' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Cardinal Allen Catholic High School, Fleetwood', N'8884718', 11, 23, 7, 0, @Now, 3051, 1205, N'SchoolCensus2026_Autumn', 7500057;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872019' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Featherby Junior School', N'8872019', 14, 0, 1, 4, @Now, 3104, 1205, N'SchoolCensus2026_Autumn', 7500058;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9414066' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Duston School', N'9414066', 1, 0, 0, 0, @Now, 3157, 1205, N'SchoolCensus2026_Autumn', 7500059;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312014' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Gagle Brook Primary School', N'9312014', 23, 0, 0, 0, @Now, 3210, 1205, N'SchoolCensus2026_Autumn', 7500060;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8672113' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Uplands Primary School and Nursery', N'8672113', 12, 0, 0, 0, @Now, 3263, 1205, N'SchoolCensus2026_Autumn', 7500061;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9412117' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Waynflete Infants'' School', N'9412117', 2, 0, 2, 5, @Now, 3316, 1205, N'SchoolCensus2026_Autumn', 7500062;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8872214' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Balfour Junior School', N'8872214', 1, 0, 0, 0, @Now, 3369, 1205, N'SchoolCensus2026_Autumn', 7500063;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8572313' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Catmose Primary', N'8572313', 1, 0, 0, 0, @Now, 3422, 1205, N'SchoolCensus2026_Autumn', 7500064;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8772416' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Newchurch Community Primary School', N'8772416', 13, 0, 0, 0, @Now, 3475, 1205, N'SchoolCensus2026_Autumn', 7500065;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9312562' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Elms Primary School', N'9312562', 2, 0, 6, 2, @Now, 3528, 1205, N'SchoolCensus2026_Autumn', 7500066;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9314560' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'The Marlborough Church of England School', N'9314560', 15, 0, 1, 5, @Now, 3581, 1205, N'SchoolCensus2026_Autumn', 7500067;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9425216' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Gilsland CofE Primary School', N'9425216', 1, 0, 0, 0, @Now, 3634, 1205, N'SchoolCensus2026_Autumn', 7500068;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'9313861' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Dunmore Primary School', N'9313861', 32, 0, 0, 0, @Now, 3687, 1205, N'SchoolCensus2026_Autumn', 7500069;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912865' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Robert Miles Junior School', N'8912865', 2, 3, 11, 0, @Now, 3740, 1205, N'SchoolCensus2026_Autumn', 7500070;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8023118' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'St Martin''s Church of England Primary School', N'8023118', 4, 0, 0, 3, @Now, 3793, 1205, N'SchoolCensus2026_Autumn', 7500071;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8912916' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Arnold Mill Primary and Nursery School', N'8912916', 1, 0, 0, 0, @Now, 3846, 1205, N'SchoolCensus2026_Autumn', 7500072;
END;
IF NOT EXISTS (SELECT 1 FROM dbo.CollectReturnStatus WHERE LAEStab = N'8812915' AND Collection = N'SchoolCensus2026_Autumn')
BEGIN
    EXEC #AddReturnStatus N'Westwood Academy', N'8812915', 31, 0, 0, 0, @Now, 3899, 1205, N'SchoolCensus2026_Autumn', 7500073;
END;
