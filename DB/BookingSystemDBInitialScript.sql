-- Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(255) CONSTRAINT UQ_Users_Email UNIQUE NOT NULL,
    Phone NVARCHAR(15) CONSTRAINT UQ_Users_Phone UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Movies Table
CREATE TABLE Movies (
    MovieId INT IDENTITY(1,1) CONSTRAINT PK_Movies PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    Language NVARCHAR(50),
    Duration INT,
    Genre NVARCHAR(200),
    ReleaseDate DATE
);

-- Trains Table
CREATE TABLE Trains (
    TrainId INT IDENTITY(1,1) CONSTRAINT PK_Trains PRIMARY KEY,
    TrainName NVARCHAR(200) NOT NULL,
    Source NVARCHAR(100) NOT NULL,
    Destination NVARCHAR(100) NOT NULL,
    DepartureDate DATE NOT NULL,
    DepartureTime TIME NOT NULL,
    ArrivalTime TIME NOT NULL
);

-- Buses Table
CREATE TABLE Buses (
    BusId INT IDENTITY(1,1) CONSTRAINT PK_Buses PRIMARY KEY,
    BusName NVARCHAR(200) NOT NULL,
    Source NVARCHAR(100) NOT NULL,
    Destination NVARCHAR(100) NOT NULL,
    DepartureDate DATE NOT NULL,
    DepartureTime TIME NOT NULL,
    ArrivalTime TIME NOT NULL
);

-- Venues Table
CREATE TABLE Venues (
    VenueId INT IDENTITY(1,1) CONSTRAINT PK_Venues PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Location NVARCHAR(200),
    TotalSeats INT NOT NULL
);

-- Seats Table
CREATE TABLE Seats (
    SeatId INT IDENTITY(1,1) CONSTRAINT PK_Seats PRIMARY KEY,
    VenueId INT NOT NULL,
    RowNumber NVARCHAR(10),
    SeatNumber NVARCHAR(10),
    Price DECIMAL(10,2),
    Status NVARCHAR(50) DEFAULT 'Available',
    CONSTRAINT FK_Seats_Venue FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

-- Events Table
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) CONSTRAINT PK_Events PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    EventDate DATE NOT NULL,
    EventTime TIME NOT NULL,
    VenueId INT NOT NULL,
    CONSTRAINT FK_Events_Venue FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

-- Shows Table
CREATE TABLE Shows (
    ShowId INT IDENTITY(1,1) CONSTRAINT PK_Shows PRIMARY KEY,
    MovieId INT NOT NULL,
    VenueId INT NOT NULL,
    ShowDate DATE NOT NULL,
    ShowTime TIME NOT NULL,
    CONSTRAINT FK_Shows_Movie FOREIGN KEY (MovieId) REFERENCES Movies(MovieId),
    CONSTRAINT FK_Shows_Venue FOREIGN KEY (VenueId) REFERENCES Venues(VenueId)
);

-- Bookings Table
CREATE TABLE Bookings (
    BookingId INT IDENTITY(1,1) CONSTRAINT PK_Bookings PRIMARY KEY,
    UserId INT NOT NULL,
    BookingType NVARCHAR(50) CONSTRAINT CK_Bookings_Type CHECK (BookingType IN ('Movie', 'Event', 'Train', 'Bus')),
    ReferenceId INT NOT NULL,
    ShowId INT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Bookings_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Bookings_Show FOREIGN KEY (ShowId) REFERENCES Shows(ShowId)
);

-- Payments Table
CREATE TABLE Payments (
    PaymentId INT IDENTITY(1,1) CONSTRAINT PK_Payments PRIMARY KEY,
    BookingId INT NOT NULL,
    UserId INT NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Status NVARCHAR(50) CONSTRAINT CK_Payments_Status CHECK (Status IN ('Success', 'Failed', 'Pending')),
    TransactionId NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Booking FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId),
    CONSTRAINT FK_Payments_User FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

-- Seat Locks Table
CREATE TABLE Seat_Locks (
    LockId INT IDENTITY(1,1) CONSTRAINT PK_SeatLocks PRIMARY KEY,
    SeatId INT NOT NULL,
    ReferenceId INT NOT NULL,
    ShowId INT NULL,
    BookingType NVARCHAR(50) CONSTRAINT CK_SeatLocks_Type CHECK (BookingType IN ('Movie', 'Event', 'Train', 'Bus')),
    UserId INT NOT NULL,
    LockedAt DATETIME DEFAULT GETDATE(),
    ExpiryTime DATETIME NOT NULL,
    CONSTRAINT FK_SeatLocks_Seat FOREIGN KEY (SeatId) REFERENCES Seats(SeatId),
    CONSTRAINT FK_SeatLocks_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_SeatLocks_Show FOREIGN KEY (ShowId) REFERENCES Shows(ShowId)
);
