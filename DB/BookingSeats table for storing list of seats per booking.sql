USE [BookingSystem]
GO

CREATE TABLE [dbo].[BookingSeats] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    BookingId INT NOT NULL,
    SeatId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),

    CONSTRAINT FK_BookingSeats_Bookings FOREIGN KEY (BookingId)
        REFERENCES [dbo].[Bookings] (BookingId),

    CONSTRAINT FK_BookingSeats_Seats FOREIGN KEY (SeatId)
        REFERENCES [dbo].[Seats] (SeatId)
);

ALTER TABLE [dbo].[BookingSeats]
ADD CONSTRAINT UQ_Seat_Per_Show UNIQUE (SeatId, BookingId);
