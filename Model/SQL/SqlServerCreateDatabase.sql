/* 
 * SQL Server Script - PracticaMAD
 * 
 * Script de creacion de la base de datos PracticaMAD.
 */

USE [master]
GO

/***** Drop database if already exists ******/
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PracticaMAD')
BEGIN
    ALTER DATABASE [PracticaMAD] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [PracticaMAD];
END
GO

/* DataBase Creation */
CREATE DATABASE [PracticaMAD]
GO

PRINT N'Database [PracticaMAD] created.'
GO

