CREATE TABLE Languages (
    LanguageId INT IDENTITY(1,1) PRIMARY KEY,
    IsoCode NVARCHAR(10) NOT NULL,
    EnglishName NVARCHAR(200) NOT NULL,
    NativeName NVARCHAR(200)
);
