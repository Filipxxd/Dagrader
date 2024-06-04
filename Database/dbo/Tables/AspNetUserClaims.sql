CREATE TABLE AspNetUserClaims (
    Id INT IDENTITY(1,1) NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    ClaimType NVARCHAR(MAX),
    ClaimValue NVARCHAR(MAX),
    CONSTRAINT PK_AspNetUserClaims PRIMARY KEY (Id),
    CONSTRAINT FK_AspNetUserClaims_AspNetUsers_UserId FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);

GO;
CREATE INDEX IX_AspNetUserClaims_UserId ON AspNetUserClaims (UserId);