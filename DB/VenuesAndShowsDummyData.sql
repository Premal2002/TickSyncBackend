--MOVIES TABLE STRUCTURE UPDATION
--USE[BookingSystem];
--GO

--ALTER TABLE[dbo].[Movies]
--ADD
--    [TMDBId] INT NULL,
--    [PosterUrl] NVARCHAR(500) NULL,
--    [BackdropUrl] NVARCHAR(500) NULL,
--    [Rating] FLOAT NULL;
--GO

--DUMMY DATA FOR VENUES AND SHOWS
--Step 1: Insert Venues
-- Insert dummy venues
--INSERT INTO Venues (Name, Location, TotalSeats) VALUES
--('PVR Cinemas', 'Mumbai', 120),
--('INOX Megaplex', 'Bangalore', 150),
--('Carnival Cinemas', 'Delhi', 100),
--('Cinepolis City Center', 'Hyderabad', 180),
--('MovieTime Arena', 'Pune', 130);

--Step 2: Insert Shows for All Movies
-- Declare helper variables
--DECLARE @MovieId INT;
--DECLARE @ShowDate DATE = CAST(GETDATE() AS DATE);
--DECLARE @VenueCount INT = (SELECT COUNT(*) FROM Venues);

---- Cursor to loop through each movie
--DECLARE MovieCursor CURSOR FOR
--SELECT MovieId FROM Movies;

--OPEN MovieCursor;
--FETCH NEXT FROM MovieCursor INTO @MovieId;

--WHILE @@FETCH_STATUS = 0
--BEGIN
--    -- Insert 3 shows per movie across different venues and times
--    INSERT INTO Shows (MovieId, VenueId, ShowDate, ShowTime)
--    SELECT @MovieId, VenueId, DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 7, @ShowDate), TimeSlot
--    FROM (
--        SELECT TOP 3 VenueId FROM Venues ORDER BY NEWID()
--    ) AS v
--    CROSS APPLY (
--        SELECT CAST('10:00:00' AS TIME) AS TimeSlot
--        UNION ALL SELECT CAST('14:00:00' AS TIME)
--        UNION ALL SELECT CAST('18:00:00' AS TIME)
--        UNION ALL SELECT CAST('21:00:00' AS TIME)
--    ) AS t
--    ORDER BY NEWID();

--    FETCH NEXT FROM MovieCursor INTO @MovieId;
--END

--CLOSE MovieCursor;
--DEALLOCATE MovieCursor;

