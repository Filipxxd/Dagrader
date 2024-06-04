CREATE TABLE [dbo].[Performances]
(
  [PerformanceId]				INT					        IDENTITY NOT NULL,
  [Value]						TINYINT						NOT NULL,
  [IsApproved]					BIT							DEFAULT (0) NOT NULL,
  [ApprovedDate]				DATETIME					NULL,
  [FeedbackMessage]				NVARCHAR (100)				NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [StudentId]					NVARCHAR (450)				NOT NULL,
  [RequiredPerformanceId]		INT					        NOT NULL,

  CONSTRAINT [PK_Performances] PRIMARY KEY CLUSTERED ([PerformanceId] ASC),
  CONSTRAINT FK_Performances_AspNetUsers_StudentId FOREIGN KEY (StudentId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE,
  CONSTRAINT FK_Performances_RequiredPerformances_RequiredPerformanceId FOREIGN KEY (RequiredPerformanceId) REFERENCES RequiredPerformances (RequiredPerformanceId) ON DELETE CASCADE,
);
GO;
CREATE NONCLUSTERED INDEX IX_Performances_Value ON Performances ([Value]);