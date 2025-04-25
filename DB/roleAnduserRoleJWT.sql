--Script to add roles and userroles tables for jwt role based authenctication and authorization

--Roles table
--CREATE TABLE [dbo].[Roles] (
--    [RoleId] INT IDENTITY(1,1) NOT NULL,
--    [RoleName] NVARCHAR(100) NOT NULL,
--    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId]),
--    CONSTRAINT [UQ_RoleName] UNIQUE ([RoleName])
--);

--UserRoles
--CREATE TABLE [dbo].[UserRoles] (
--    [UserId] INT NOT NULL,
--    [RoleId] INT NOT NULL,
--    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
--    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
--    CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([RoleId]) ON DELETE CASCADE
--);

----Drop the existing primary key (if it exists):
--ALTER TABLE UserRoles
--DROP CONSTRAINT PK_UserRoles;
---- Step 1: Add the ID column
--ALTER TABLE UserRoles
--ADD ID INT IDENTITY(1,1);

---- Step 2: Set the new ID column as primary key
--ALTER TABLE UserRoles
--ADD CONSTRAINT PK_UserRoles_ID PRIMARY KEY (ID);