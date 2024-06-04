CREATE TABLE [dbo].[RequiredPerformances]
(
  [RequiredPerformanceId]		INT					        IDENTITY NOT NULL,
  [EntryStartDate]				DATETIME					NOT NULL,
  [FeedbackMessage]				NVARCHAR (100)				NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [CourseId]					INT					        NULL,
  [ActivityId]					INT					        NOT NULL,

  CONSTRAINT [PK_RequiredPerformances] PRIMARY KEY CLUSTERED ([RequiredPerformanceId] ASC),
  CONSTRAINT FK_RequiredPerformances_Courses_CourseId FOREIGN KEY (CourseId) REFERENCES Courses (CourseId) ON DELETE SET NULL,
  CONSTRAINT FK_RequiredPerformances_Activities_ActivityId FOREIGN KEY (ActivityId) REFERENCES Activities (ActivityId) ON DELETE CASCADE,
);