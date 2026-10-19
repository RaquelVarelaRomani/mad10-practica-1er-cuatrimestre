/* 
 * SQL Server Script - PracticaMAD
 * 
 * Script de creacion de tablas e insercion de datos iniciales.
 * Base de datos: PracticaMAD
 */

USE [PracticaMAD]
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


/* ************************************************************************** */
/* ****************** INSERCION DE DATOS INICIALES ************************** */
/* ************************************************************************** */

/* 1. Usuarios iniciales (vendedores y compradores) */
/* Nota: Las contrasenas pueden generarse o actualizarse mediante PasswordEncrypter */
INSERT INTO UserProfile (loginName, enPassword, firstName, lastName, email, language, country)
VALUES 
('vendedor1', 'passwordHash1', 'Carlos', 'Garcia Perez', 'carlos.vendedor@udc.es', 'es', 'ES'),
('vendedor2', 'passwordHash2', 'Marta', 'Lopez Gomez', 'marta.vendedor@udc.es', 'es', 'ES'),
('comprador1', 'passwordHash3', 'Ana', 'Rodriguez Silva', 'ana.comprador@udc.es', 'es', 'ES'),
('comprador2', 'passwordHash4', 'David', 'Fernandez Vazquez', 'david.comprador@udc.es', 'en', 'US')
GO

/* 2. Categorias del catalogo (sistema plano) */
INSERT INTO Category (categoryName)
VALUES 
('Electronica'),
('Informatica'),
('Libros'),
('Hogar y Cocina'),
('Deportes')
GO

/* 3. Productos en el catalogo */
-- Asumimos IDs 1 y 2 para vendedores y 1 a 5 para categorias
INSERT INTO Product (productName, creationDate, description, image, categoryId, sellerId)
VALUES 
('Portatil Lenovo ThinkPad T14', GETDATE(), 'Intel Core i7, 16GB RAM, 512GB SSD NVMe, pantalla FHD IPS', 'lenovo_t14.jpg', 2, 1),
('Smartphone Samsung Galaxy S23', GETDATE(), '128GB Almacenamiento, Pantalla AMOLED 120Hz, Triple Camara 50MP', 'samsung_s23.jpg', 1, 1),
('Auriculares Inalambricos Sony WH-1000XM5', GETDATE(), 'Cancelacion de ruido activa lider en la industria, bateria 30h', 'sony_xm5.jpg', 1, 2),
('Libro Clean Code - Robert C. Martin', GETDATE(), 'Guia clasica de buenas practicas y desarrollo de software agil', 'clean_code.jpg', 3, 2),
('Libro Design Patterns - Gang of Four', GETDATE(), 'Elementos de software reutilizable orientado a objetos', 'design_patterns.jpg', 3, 2),
('Monitor Dell UltraSharp 27 pulgadas 4K', GETDATE(), 'Resolucion 3840x2160, panel IPS, 99% sRGB, conectividad USB-C', 'dell_u27.jpg', 2, 1),
('Teclado Mecanico Inalambrico Keychron K2', GETDATE(), 'Switches Gateron Brown, retroiluminacion RGB, layout 75%', 'keychron_k2.jpg', 2, 2),
('Cafetera Superautomatica DeLonghi', GETDATE(), 'Molinillo integrado, 15 bares de presion, espumador de leche', 'delonghi.jpg', 4, 1),
('Bicicleta de Montana Rockrider ST 540', GETDATE(), 'Ruedas 27.5 pulgadas, frenos de disco hidraulicos, 18 velocidades', 'rockrider.jpg', 5, 2)
GO

/* 4. Comentarios iniciales sobre productos */
-- Asumimos producto 1 (Lenovo ThinkPad) y compradores 3 y 4
INSERT INTO Comment (productId, userId, commentText, commentDate, isInReview)
VALUES 
(1, 3, 'Excelente rendimiento para desarrollo de software y bateria muy duradera.', DATEADD(day, -5, GETDATE()), 0),
(1, 4, 'El teclado es comodisimo, muy satisfecho con la compra.', DATEADD(day, -2, GETDATE()), 0),
(3, 3, 'La cancelacion de ruido es increible en viajes de avion.', DATEADD(day, -3, GETDATE()), 0),
(4, 4, 'Lectura obligatoria para cualquier ingeniero de software.', DATEADD(day, -10, GETDATE()), 0)
GO

/* 5. Etiquetas iniciales (Opcional 1) */
INSERT INTO Tag (tagName)
VALUES 
('ganga'),
('calidad'),
('recomendado'),
('rapido'),
('pesimo'),
('oferta')
GO

/* 6. Asociacion de comentarios con etiquetas */
-- Asumimos commentId 1, 2, 3, 4 y tagId 1 a 6
INSERT INTO CommentTag (commentId, tagId)
VALUES 
(1, 2), -- Calidad
(1, 3), -- Recomendado
(2, 3), -- Recomendado
(3, 2), -- Calidad
(4, 3)  -- Recomendado
GO

/* 7. Valoraciones iniciales a vendedores */
-- vendedor 1 (usrId 1) valorado por comprador 3 (usrId 3) respecto al producto 1
INSERT INTO SellerRating (sellerId, productId, buyerUserId, score, comment, ratingDate)
VALUES 
(1, 1, 3, 5, 'Envio rapidisimo y el portatil venia en perfecto estado con embalaje original.', DATEADD(day, -4, GETDATE())),
(1, 2, 4, 4, 'Todo correcto, buena comunicacion aunque tardo un dia mas en llegar.', DATEADD(day, -1, GETDATE())),
(2, 3, 3, 5, 'Vendedor 100% recomendable, responde dudas al instante.', DATEADD(day, -2, GETDATE()))
GO

PRINT N'Tablas y datos iniciales creados exitosamente en [PracticaMAD].'
GO

