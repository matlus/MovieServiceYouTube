CREATE VIEW [dbo].[MovieVw]
AS
	SELECT
			dbo.Movie.Id,
			dbo.Movie.Title,
			dbo.Genre.Title AS Genre,
			dbo.Movie.Year,
			dbo.Movie.ImageUrl
	FROM	dbo.Movie
	INNER JOIN dbo.Assoc_MovieGenre ON dbo.Movie.Id = dbo.Assoc_MovieGenre.MovieId
	INNER JOIN dbo.Genre ON dbo.Genre.Id = dbo.Assoc_MovieGenre.GenreId