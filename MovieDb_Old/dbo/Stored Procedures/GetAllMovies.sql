CREATE PROCEDURE [dbo].[GetAllMovies]
AS
	SET NOCOUNT ON
	SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw

RETURN 0