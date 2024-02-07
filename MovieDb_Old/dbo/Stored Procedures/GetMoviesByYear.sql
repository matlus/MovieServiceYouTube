CREATE PROCEDURE [dbo].[GetMoviesByYear]
	@Year int
AS
	SET NOCOUNT ON	
	SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw
	WHERE	Year = @Year

RETURN 0