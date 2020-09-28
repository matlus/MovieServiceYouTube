/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET	IDENTITY_INSERT [dbo].[Movie] ON
INSERT
INTO	[dbo].[Movie]
		([Id], [Title], [Year], [ImageUrl])
VALUES	(1, 'This is the Title for Movie 1', 1999, 'http://www.imdb.com/images/1'),
		(2, 'This is the Title for Movie 2', 2000, 'http://www.imdb.com/images/2'),
		(3, 'This is the Title for Movie 3', 2001, 'http://www.imdb.com/images/3'),
		(4, 'This is the Title for Movie 4', 2002, 'http://www.imdb.com/images/4'),
		(5, 'This is the Title for Movie 5', 2003, 'http://www.imdb.com/images/5'),
		(6, 'This is the Title for Movie 6', 2004, 'http://www.imdb.com/images/6'),
		(7, 'This is the Title for Movie 7', 2005, 'http://www.imdb.com/images/7'),
		(8, 'This is the Title for Movie 8', 2006, 'http://www.imdb.com/images/8'),
		(9, 'This is the Title for Movie 9', 2007, 'http://www.imdb.com/images/9'),
		(10, 'This is the Title for Movie 10', 2008, 'http://www.imdb.com/images/10')
SET	IDENTITY_INSERT [dbo].[Movie] OFF


SET	IDENTITY_INSERT [dbo].[Genre] ON
INSERT
INTO	[dbo].[Genre]
		([Id], [Title])
VALUES	(1, 'Action'),
		(2, 'Comedy'),
		(3, 'Drama'),
		(4, 'Sci-Fi'),
		(5, 'Thriller')
SET	IDENTITY_INSERT [dbo].[Genre] OFF



INSERT
INTO	[dbo].[Assoc_MovieGenre]
		([MovieId], [GenreId])
VALUES	(1, 1),
		(2, 2),
		(3, 3),
		(4, 4),
		(5, 5),
		(6, 1),
		(7, 2),
		(8, 3),
		(9, 4),
		(10, 5)