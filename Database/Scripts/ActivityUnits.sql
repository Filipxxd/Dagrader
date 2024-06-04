IF OBJECT_Id('tempdb..#ActivityUnits') IS NOT NULL
    DROP TABLE #ActivityUnits;

CREATE TABLE #ActivityUnits
(
    [ActivityUnitId] INT,
	[DisplayName] NVARCHAR(64),
	[DisplayNameShort] NVARCHAR(64),
	[IsDeletable] BIT,
	[DateCreated] DATETIME,
	[DateUpdated] DATETIME,
);
INSERT INTO #ActivityUnits
VALUES
    (1, N'Sekund', N's', 0, GETDATE(), GETDATE()),
    (2, N'Metrů', N'm', 0, GETDATE(), GETDATE());


SET IdENTITY_INSERT [dbo].[ActivityUnits] ON;

MERGE [dbo].[ActivityUnits] AS [Target]
USING #ActivityUnits AS [Source]
ON [Target].[ActivityUnitId] = [Source].[ActivityUnitId]
WHEN MATCHED THEN
    UPDATE SET
        [Target].[DisplayName] = [Source].[DisplayName],
        [Target].[DisplayNameShort] = [Source].[DisplayNameShort],
		[Target].[IsDeletable] = [Source].[IsDeletable],
		[Target].[DateCreated] = [Source].[DateCreated],
		[Target].[DateUpdated] = [Source].[DateUpdated]
WHEN NOT MATCHED THEN
    INSERT ([ActivityUnitId], [DisplayName], [DisplayNameShort], [IsDeletable], [DateCreated], [DateUpdated])
    VALUES ([Source].[ActivityUnitId], [Source].[DisplayName], [Source].[DisplayNameShort], [Source].[IsDeletable], [Source].[DateCreated], [Source].[DateUpdated]);

SET IdENTITY_INSERT [dbo].[ActivityUnits] OFF;

DROP TABLE #ActivityUnits;
