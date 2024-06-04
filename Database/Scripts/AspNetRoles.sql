IF OBJECT_ID('tempdb..#AspNetRoles') IS NOT NULL
    DROP TABLE #AspNetRoles;

CREATE TABLE #AspNetRoles
(
    [Id] NVARCHAR(512) COLLATE SQL_Latin1_General_CP1_CI_AS,
    [Name] NVARCHAR(64) COLLATE SQL_Latin1_General_CP1_CI_AS,
    [NormalizedName] NVARCHAR(64) COLLATE SQL_Latin1_General_CP1_CI_AS,
    [ConcurrencyStamp] NVARCHAR(512) COLLATE SQL_Latin1_General_CP1_CI_AS
);

INSERT INTO #AspNetRoles
VALUES
('D2A35AEC-402A-4BE6-9D7F-682C0B5D3FEF', 'Administrator', 'ADMINISTRATOR', 'BD480BB1-7D73-4393-AA09-51B11AAC949E'),
('642A2EBD-1B2F-4321-B294-028C3A27E0A2', 'Student', 'STUDENT', '1FE5B345-A5E0-496E-8956-B48D21AA37CC'),
('B6B89D1B-02D5-46D3-9BB4-8F1E99AA934C', 'Teacher', 'TEACHER', '77ABAEAD-5A6E-4D27-B9E6-A080E64EFBD6');

MERGE [dbo].[AspNetRoles] AS [Target]
USING #AspNetRoles AS [Source]
ON [Target].[Id] = [Source].[Id]
WHEN MATCHED THEN
    UPDATE SET
        [Target].[Name] = [Source].[Name],
        [Target].[NormalizedName] = [Source].[NormalizedName],
        [Target].[ConcurrencyStamp] = [Source].[ConcurrencyStamp]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [ConcurrencyStamp], [NormalizedName])
    VALUES ([Source].[Id], [Source].[Name], [Source].[ConcurrencyStamp], [Source].[NormalizedName]);

DROP TABLE #AspNetRoles;
