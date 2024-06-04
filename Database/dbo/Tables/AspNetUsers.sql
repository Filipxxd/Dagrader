CREATE TABLE AspNetUsers (
    Id NVARCHAR(450) NOT NULL,
    UserName NVARCHAR(256),
    NormalizedUserName NVARCHAR(256),
    Email NVARCHAR(256),
    NormalizedEmail NVARCHAR(256),
    EmailConfirmed BIT NOT NULL,
    PasswordHash NVARCHAR(MAX),
    SecurityStamp NVARCHAR(MAX),
    ConcurrencyStamp NVARCHAR(MAX),
    PhoneNumber NVARCHAR(MAX),
    PhoneNumberConfirmed BIT NOT NULL,
    TwoFactorEnabled BIT NOT NULL,
    LockoutEnd DATETIMEOFFSET,
    LockoutEnabled BIT NOT NULL,
    AccessFailedCount INT NOT NULL,
	FirstName NVARCHAR(256) NOT NULL,
    LastName NVARCHAR(256) NOT NULL,
    Gender INT NOT NULL,
	CourseId INT NULL,
    PRIMARY KEY (Id),
	CONSTRAINT FK_AspNetUsers_Courses_CourseId FOREIGN KEY (CourseId) REFERENCES Courses (CourseId) ON DELETE SET NULL
);
GO;
CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedEmail ON AspNetUsers (NormalizedEmail);
GO;
CREATE UNIQUE INDEX IX_AspNetUsers_NormalizedUserName ON AspNetUsers (NormalizedUserName) WHERE NormalizedUserName IS NOT NULL;
GO;
CREATE UNIQUE INDEX IX_AspNetUsers_FirstName ON AspNetUsers (FirstName);
GO;
CREATE UNIQUE INDEX IX_AspNetUsers_LastName ON AspNetUsers (LastName);