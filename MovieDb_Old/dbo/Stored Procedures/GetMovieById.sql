CREATE PROCEDURE [dbo].[GetMovieById]
	@Id int
AS
	SET NOCOUNT ON
	SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw
	WHERE	Id = @Id

RETURN 0