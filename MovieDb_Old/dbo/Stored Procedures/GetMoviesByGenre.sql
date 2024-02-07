CREATE PROCEDURE [dbo].[GetMoviesByGenre]
	@Genre varchar(50)
AS
	SET NOCOUNT ON
	SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw
	WHERE	Genre = @Genre

RETURN 0