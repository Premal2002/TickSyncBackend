-- Drop existing table if exists (use with caution in dev only)
DROP TABLE IF EXISTS [dbo].[Payments];

-- Create updated Payments table
CREATE TABLE [dbo].[Payments] (
    [PaymentId] INT IDENTITY(1,1) PRIMARY KEY,
    [BookingId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Amount] DECIMAL(10,2) NOT NULL,
    [Currency] NVARCHAR(10) DEFAULT 'INR',
    [Status] NVARCHAR(50),                      -- e.g., 'Success', 'Failed', 'Pending'
    [RazorpayOrderId] NVARCHAR(100),            -- Generated when order is created
    [RazorpayPaymentId] NVARCHAR(100),          -- Returned after payment success
    [RazorpaySignature] NVARCHAR(255),          -- For validation
    [PaymentMethod] NVARCHAR(50),               -- Optional (e.g., card, upi, etc.)
    [TransactionId] NVARCHAR(100),              -- Optional / Internal reference
    [Notes] NVARCHAR(MAX),                      -- Optional for debugging/logging
    [CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
    [PaidAt] DATETIME NULL,                     -- Time of actual success

    -- Foreign key constraints
    CONSTRAINT FK_Payments_Bookings FOREIGN KEY ([BookingId]) REFERENCES [dbo].[Bookings]([BookingId]),
    CONSTRAINT FK_Payments_Users FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
);
