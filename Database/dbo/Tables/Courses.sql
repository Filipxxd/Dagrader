CREATE TABLE [dbo].[Courses]
(
  [CourseId]					INT					        IDENTITY NOT NULL,
  [AcademicYear]				NVARCHAR (9)				NOT NULL,
  [ClassYear]					TINYINT						NOT NULL,
  [ClassSymbol]					CHAR						NOT NULL,
  
  [CreatedDate]					DATETIME					NOT NULL,
  [UpdatedDate]					DATETIME					NOT NULL,
  [TeacherId]					NVARCHAR (450)				NOT NULL,

  CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([CourseId] ASC),
  CONSTRAINT FK_Courses_AspNetUsers_TeacherId FOREIGN KEY (TeacherId) REFERENCES AspNetUsers (Id)
);