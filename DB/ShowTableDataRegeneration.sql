--DELETE FROM [BookingSystem].[dbo].[Payments]
--DELETE  FROM [BookingSystem].[dbo].[BookingSeats]
--DELETE FROM [BookingSystem].[dbo].[Bookings]
--DELETE FROM [BookingSystem].[dbo].[Shows]

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

--UPDATE s
--SET 
--    s.RegularSeatPrice = 
--        CASE 
--            WHEN v.Name = 'PVR Cinemas' THEN 120  -- Specific price for PVR Cinemas
--            WHEN v.Name = 'INOX Megaplex' THEN 110  -- Specific price for INOX Megaplex
--            ELSE 100  -- Default price
--        END,
--    s.PremiumSeatPrice = 
--        CASE 
--            WHEN v.Name = 'PVR Cinemas' THEN 170  -- Specific price for Premium seats at PVR Cinemas
--            WHEN v.Name = 'INOX Megaplex' THEN 160  -- Specific price for Premium seats at INOX Megaplex
--            ELSE 150  -- Default Premium price
--        END
--FROM Shows s
--JOIN Venues v ON s.VenueId = v.VenueId;
