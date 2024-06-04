CREATE TABLE [dbo].[Activities]
(
  [ActivityId]					INT					        IDENTITY NOT NULL,
  [DisplayName]					NVARCHAR (64)				NOT NULL,
  [Icon]						NVARCHAR (2048)				NOT NULL,
  [IsDeletable]				    BIT					        DEFAULT (1) NOT NULL,
  [IsGreaterBetter]				BIT					        DEFAULT (1) NOT NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [ActivityUnitId]				INT							NOT NULL,

  CONSTRAINT [PK_Activities] PRIMARY KEY CLUSTERED ([ActivityId] ASC),
  CONSTRAINT FK_Activities_ActivityUnits_ActivityUnitId FOREIGN KEY (ActivityUnitId) REFERENCES ActivityUnits (ActivityUnitId) ON DELETE CASCADE
);