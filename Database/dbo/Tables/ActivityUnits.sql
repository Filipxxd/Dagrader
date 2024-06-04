CREATE TABLE [dbo].[ActivityUnits]
(
  [ActivityUnitId]				INT					        IdENTITY NOT NULL,
  [DisplayName]					NVARCHAR (64)				NOT NULL,
  [DisplayNameShort]			NVARCHAR (16)				NOT NULL,
  [IsDeletable]				    BIT					        DEFAULT (1) NOT NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,

  CONSTRAINT [PK_ActivityUnits] PRIMARY KEY CLUSTERED ([ActivityUnitId] ASC)
);