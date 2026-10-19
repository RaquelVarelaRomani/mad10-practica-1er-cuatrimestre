/* 
 * SQL Server Script - PracticaMAD
 * 
 * Script de creacion de la base de datos PracticaMAD.
 * 
 * En un entorno local (p. ej. LocalDB o SQL Server Express) configure
 * la variable @Default_DB_Path con la ruta deseada para almacenar los archivos .mdf y .ldf.
 */

DECLARE @Default_DB_Path as VARCHAR(255)  
SET @Default_DB_Path = N'C:\SourceCode.SVN\DataBase\'
 
USE [master]
GO

/***** Drop database if already exists ******/
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PracticaMAD')
BEGIN
    ALTER DATABASE [PracticaMAD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [PracticaMAD];
END
GO

USE [master]
GO

/* DataBase Creation */
DECLARE @sql nvarchar(1000)

SET @sql = 
  N'CREATE DATABASE [PracticaMAD] 
    ON PRIMARY ( NAME = PracticaMAD, FILENAME = ''' + @Default_DB_Path + N'PracticaMAD.mdf'')
    LOG ON ( NAME = PracticaMAD_log, FILENAME = ''' + @Default_DB_Path + N'PracticaMAD_log.ldf'')'

EXEC(@sql)
PRINT N'Database [PracticaMAD] created.'
GO

