CREATE TABLE [dbo].[Grades]
(
  [GradeId]						INT					        IDENTITY NOT NULL,
  [Value]						TINYINT						NOT NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [PerformanceId]				INT							NOT NULL,

  CONSTRAINT [PK_Grades] PRIMARY KEY CLUSTERED ([GradeId] ASC),
  CONSTRAINT FK_Grades_Performances_PerformanceId FOREIGN KEY (PerformanceId) REFERENCES Performances (PerformanceId) ON DELETE CASCADE
);