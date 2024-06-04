CREATE TABLE AspNetRoles (
    Id NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(256),
    NormalizedName NVARCHAR(256),
    ConcurrencyStamp NVARCHAR(MAX),
    PRIMARY KEY (Id)
);

GO;
CREATE UNIQUE INDEX RoleNameIndex ON AspNetRoles (NormalizedName) WHERE NormalizedName IS NOT NULL;