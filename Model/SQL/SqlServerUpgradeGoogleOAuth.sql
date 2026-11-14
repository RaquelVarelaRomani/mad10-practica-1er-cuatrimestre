/* Run against the selected PracticaMAD or PracticaMAD_test database.
   This migration preserves existing users and can be run more than once. */
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO

IF COL_LENGTH('dbo.UserProfile', 'googleSubject') IS NULL
BEGIN
    ALTER TABLE dbo.UserProfile ADD googleSubject varchar(255) COLLATE Latin1_General_100_BIN2 NULL;
END
ELSE IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.UserProfile')
               AND name = 'googleSubject' AND collation_name <> 'Latin1_General_100_BIN2')
BEGIN
    ALTER TABLE dbo.UserProfile ALTER COLUMN googleSubject varchar(255) COLLATE Latin1_General_100_BIN2 NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE object_id = OBJECT_ID('dbo.UserProfile')
                 AND name = 'UQ_UserProfile_GoogleSubject')
BEGIN
    CREATE UNIQUE INDEX UQ_UserProfile_GoogleSubject
        ON dbo.UserProfile(googleSubject) WHERE googleSubject IS NOT NULL;
END
GO
