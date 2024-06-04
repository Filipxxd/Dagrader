CREATE TABLE [dbo].[Criterias]
(
  [CriteriaId]					INT					        IDENTITY NOT NULL,
  [DisplayName]					NVARCHAR (64)				NOT NULL,
  [ClassYear]					TINYINT						NOT NULL,
  [Male_1]						DECIMAL						NOT NULL,
  [Male_2]						DECIMAL						NOT NULL,
  [Male_3]						DECIMAL						NOT NULL,
  [Male_4]						DECIMAL						NOT NULL,
  [Female_1]					DECIMAL						NOT NULL,
  [Female_2]					DECIMAL						NOT NULL,
  [Female_3]					DECIMAL						NOT NULL,
  [Female_4]					DECIMAL						NOT NULL,

  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [ActivityId]					INT							NOT NULL,
  [TeacherId]					NVARCHAR (450)				NOT NULL,

  CONSTRAINT [PK_Criterias] PRIMARY KEY CLUSTERED ([CriteriaId] ASC),
  CONSTRAINT FK_Criterias_Activities_ActivityId FOREIGN KEY (ActivityId) REFERENCES Activities (ActivityId) ON DELETE CASCADE,
  CONSTRAINT FK_Criterias_AspNetUsers_TeacherId FOREIGN KEY (TeacherId) REFERENCES AspNetUsers (Id) ON DELETE CASCADE
);