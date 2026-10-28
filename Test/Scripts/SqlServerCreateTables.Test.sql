/* 
 * SQL Server Script - PracticaMAD_test
 * 
 * Script de creacion de tablas para la base de datos de pruebas (Test).
 * Base de datos: PracticaMAD_test
 */

USE [master]
GO

/***** Drop database if already exists  ******/
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'PracticaMAD_test')
BEGIN
    ALTER DATABASE [PracticaMAD_test] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [PracticaMAD_test];
END
GO

/* DataBase Creation */
CREATE DATABASE [PracticaMAD_test]
GO

PRINT N'Database [PracticaMAD_test] created.'
GO

USE [PracticaMAD_test]
GO

/* ********** Drop tables in reverse dependency order *********** */

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[CommentTag]') AND type in ('U'))
DROP TABLE [CommentTag]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[Tag]') AND type in ('U'))
DROP TABLE [Tag]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[CommentReport]') AND type in ('U'))
DROP TABLE [CommentReport]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[Comment]') AND type in ('U'))
DROP TABLE [Comment]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[SellerRating]') AND type in ('U'))
DROP TABLE [SellerRating]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[Product]') AND type in ('U'))
DROP TABLE [Product]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[Category]') AND type in ('U'))
DROP TABLE [Category]
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID('[UserProfile]') AND type in ('U'))
DROP TABLE [UserProfile]
GO


/* ********** 1. UserProfile *********** */

CREATE TABLE UserProfile (
	usrId bigint IDENTITY(1,1) NOT NULL,
	loginName varchar(30) NOT NULL,
	enPassword varchar(50) NOT NULL,
	firstName varchar(30) NOT NULL,
	lastName varchar(40) NOT NULL,
	email varchar(60) NOT NULL,
	language varchar(2) NOT NULL,
	country varchar(2) NOT NULL,
	isExternal BIT NOT NULL DEFAULT 0,

	CONSTRAINT [PK_UserProfile] PRIMARY KEY (usrId),
	CONSTRAINT [UQ_UserProfile_LoginName] UNIQUE (loginName)
)
GO

CREATE NONCLUSTERED INDEX [IX_UserProfile_LoginName]
ON [UserProfile] ([loginName] ASC)
GO


/* ********** 2. Category *********** */

CREATE TABLE Category (
	categoryId bigint IDENTITY(1,1) NOT NULL,
	categoryName varchar(50) NOT NULL,

	CONSTRAINT [PK_Category] PRIMARY KEY (categoryId),
	CONSTRAINT [UQ_Category_CategoryName] UNIQUE (categoryName)
)
GO


/* ********** 3. Product *********** */

CREATE TABLE Product (
	productId bigint IDENTITY(1,1) NOT NULL,
	productName varchar(150) NOT NULL,
	creationDate datetime NOT NULL,
	description varchar(1000) NULL,
	image varchar(255) NULL,
	categoryId bigint NOT NULL,
	sellerId bigint NOT NULL,

	CONSTRAINT [PK_Product] PRIMARY KEY (productId),
	CONSTRAINT [FK_Product_Category] FOREIGN KEY (categoryId) REFERENCES Category(categoryId),
	CONSTRAINT [FK_Product_Seller] FOREIGN KEY (sellerId) REFERENCES UserProfile(usrId)
)
GO

CREATE NONCLUSTERED INDEX [IX_Product_Name] ON Product ([productName] ASC)
CREATE NONCLUSTERED INDEX [IX_Product_Category] ON Product ([categoryId] ASC)
CREATE NONCLUSTERED INDEX [IX_Product_Seller] ON Product ([sellerId] ASC)
GO


/* ********** 4. Comment *********** */

CREATE TABLE Comment (
	commentId bigint IDENTITY(1,1) NOT NULL,
	productId bigint NOT NULL,
	userId bigint NOT NULL,
	commentText varchar(1000) NOT NULL,
	commentDate datetime NOT NULL,
	isInReview bit NOT NULL CONSTRAINT [DF_Comment_IsInReview] DEFAULT (0),

	CONSTRAINT [PK_Comment] PRIMARY KEY (commentId),
	CONSTRAINT [FK_Comment_Product] FOREIGN KEY (productId) REFERENCES Product(productId) ON DELETE CASCADE,
	CONSTRAINT [FK_Comment_User] FOREIGN KEY (userId) REFERENCES UserProfile(usrId)
)
GO

CREATE NONCLUSTERED INDEX [IX_Comment_Product_Date] ON Comment ([productId] ASC, [commentDate] DESC)
CREATE NONCLUSTERED INDEX [IX_Comment_User] ON Comment ([userId] ASC)
GO


/* ********** 5. CommentReport *********** */

CREATE TABLE CommentReport (
	reportId bigint IDENTITY(1,1) NOT NULL,
	commentId bigint NOT NULL,
	userId bigint NOT NULL,
	reason varchar(255) NOT NULL,
	reportDate datetime NOT NULL,

	CONSTRAINT [PK_CommentReport] PRIMARY KEY (reportId),
	CONSTRAINT [FK_CommentReport_Comment] FOREIGN KEY (commentId) REFERENCES Comment(commentId) ON DELETE CASCADE,
	CONSTRAINT [FK_CommentReport_User] FOREIGN KEY (userId) REFERENCES UserProfile(usrId),
	CONSTRAINT [UQ_CommentReport_Comment_User] UNIQUE (commentId, userId)
)
GO

CREATE NONCLUSTERED INDEX [IX_CommentReport_Comment] ON CommentReport ([commentId] ASC)
GO


/* ********** 6. SellerRating *********** */

CREATE TABLE SellerRating (
	ratingId bigint IDENTITY(1,1) NOT NULL,
	sellerId bigint NOT NULL,
	productId bigint NOT NULL,
	buyerUserId bigint NOT NULL,
	score int NOT NULL,
	comment varchar(1000) NULL,
	ratingDate datetime NOT NULL,

	CONSTRAINT [PK_SellerRating] PRIMARY KEY (ratingId),
	CONSTRAINT [FK_SellerRating_Seller] FOREIGN KEY (sellerId) REFERENCES UserProfile(usrId),
	CONSTRAINT [FK_SellerRating_Product] FOREIGN KEY (productId) REFERENCES Product(productId),
	CONSTRAINT [FK_SellerRating_Buyer] FOREIGN KEY (buyerUserId) REFERENCES UserProfile(usrId),
	CONSTRAINT [CK_SellerRating_Score] CHECK (score >= 1 AND score <= 5),
	CONSTRAINT [CK_SellerRating_NotSelf] CHECK (sellerId <> buyerUserId)
)
GO

CREATE NONCLUSTERED INDEX [IX_SellerRating_Seller_Date] ON SellerRating ([sellerId] ASC, [ratingDate] DESC)
CREATE NONCLUSTERED INDEX [IX_SellerRating_Product] ON SellerRating ([productId] ASC)
GO


/* ********** 7. Tag (Opcional 1: Etiquetado) *********** */

CREATE TABLE Tag (
	tagId bigint IDENTITY(1,1) NOT NULL,
	tagName varchar(50) NOT NULL,

	CONSTRAINT [PK_Tag] PRIMARY KEY (tagId),
	CONSTRAINT [UQ_Tag_TagName] UNIQUE (tagName)
)
GO

CREATE TABLE CommentTag (
	commentId bigint NOT NULL,
	tagId bigint NOT NULL,

	CONSTRAINT [PK_CommentTag] PRIMARY KEY (commentId, tagId),
	CONSTRAINT [FK_CommentTag_Comment] FOREIGN KEY (commentId) REFERENCES Comment(commentId) ON DELETE CASCADE,
	CONSTRAINT [FK_CommentTag_Tag] FOREIGN KEY (tagId) REFERENCES Tag(tagId) ON DELETE CASCADE
)
GO

CREATE NONCLUSTERED INDEX [IX_CommentTag_Tag] ON CommentTag ([tagId] ASC)
GO

PRINT N'Tablas creadas exitosamente en [PracticaMAD_test].'
GO

