
--Delete seat_Locks Table
drop table dbo.Seat_Locks

--Delete seats Table
drop table dbo.Seats

--Create seats table with new structure
CREATE TABLE Seats (
    SeatId INT IDENTITY(1,1) CONSTRAINT PK_Seats PRIMARY KEY,
    VenueId INT NOT NULL,
    RowNumber NVARCHAR(10) NOT NULL,      -- Example: A, B, C or 1, 2
    SeatNumber NVARCHAR(10) NOT NULL,     -- Example: 1, 2, 3, 10
    SeatType NVARCHAR(50) NOT NULL,       -- Example: Regular, VIP, Premium
    Status NVARCHAR(50) DEFAULT 'Available',  -- General status (can be used for maintenance or disabled seats)
    CONSTRAINT FK_Seats_Venue FOREIGN KEY (VenueId) REFERENCES Venues(VenueId),
    CONSTRAINT UQ_Seat_RowSeat UNIQUE (VenueId, RowNumber, SeatNumber) -- to avoid duplicates in same venue
);


--Script to generate seats data for all venues(Same layout for all venues)
-- Define the seat layout and insert for each venue
DECLARE @VenueId INT;
DECLARE @Row CHAR(1);
DECLARE @SeatNum INT;
DECLARE @SeatType NVARCHAR(50);

-- Cursor to loop through all venues
DECLARE venue_cursor CURSOR FOR
SELECT VenueId FROM Venues;

OPEN venue_cursor;
FETCH NEXT FROM venue_cursor INTO @VenueId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Loop through rows A to F
    SET @Row = 'A';
    WHILE @Row <= 'F'
    BEGIN
        -- Set seat type
        IF @Row = 'F'
            SET @SeatType = 'Premium';
        ELSE
            SET @SeatType = 'Regular';

        -- Insert 10 seats per row
        SET @SeatNum = 1;
        WHILE @SeatNum <= 10
        BEGIN
            INSERT INTO Seats (VenueId, RowNumber, SeatNumber, SeatType)
            VALUES (@VenueId, @Row, CAST(@SeatNum AS NVARCHAR), @SeatType);

            SET @SeatNum = @SeatNum + 1;
        END

        -- Move to next row
        SET @Row = CHAR(ASCII(@Row) + 1);
    END

    FETCH NEXT FROM venue_cursor INTO @VenueId;
END

CLOSE venue_cursor;
DEALLOCATE venue_cursor;


--Alter shows table to store prices of the seats
ALTER TABLE [dbo].[Shows]
ADD
    RegularSeatPrice DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    PremiumSeatPrice DECIMAL(10,2) NOT NULL DEFAULT 0.00;


--Adding prices for all the shows based on venues (simple logic based on venues)
-- Update Regular and Premium Seat Prices based on Movie or Venue
UPDATE s
SET 
    s.RegularSeatPrice = 
        CASE 
            WHEN v.Name = 'PVR Cinemas' THEN 120  -- Specific price for PVR Cinemas
            WHEN v.Name = 'INOX Megaplex' THEN 110  -- Specific price for INOX Megaplex
            ELSE 100  -- Default price
        END,
    s.PremiumSeatPrice = 
        CASE 
            WHEN v.Name = 'PVR Cinemas' THEN 170  -- Specific price for Premium seats at PVR Cinemas
            WHEN v.Name = 'INOX Megaplex' THEN 160  -- Specific price for Premium seats at INOX Megaplex
            ELSE 150  -- Default Premium price
        END
FROM Shows s
JOIN Venues v ON s.VenueId = v.VenueId;

