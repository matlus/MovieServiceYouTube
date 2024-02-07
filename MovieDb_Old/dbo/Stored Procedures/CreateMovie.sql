CREATE PROCEDURE [dbo].[CreateMovie]
	@Title varchar(50),
	@Genre varchar(50),
	@Year int,
	@ImageUrl varchar(200)
AS
	SET XACT_ABORT ON
	SET NOCOUNT ON

	INSERT INTO dbo.Movie
	(Title, Year, ImageUrl)
	VALUES(@Title, @Year, @ImageUrl)
	
	DECLARE @MovieId int = SCOPE_IDENTITY()
	DECLARE @GenreId int;

	SELECT @GenreId = dbo.Genre.Id FROM dbo.Genre WHERE dbo.Genre.Title = @Genre

	IF (@GenreId IS NULL)
	BEGIN
		INSERT INTO dbo.Genre
		(Title)
		VALUES(@Genre)

		SET @GenreId = SCOPE_IDENTITY()
	END
	
	INSERT INTO dbo.Assoc_MovieGenre
	(MovieId, GenreId)
	VALUES(@MovieId, @GenreId)

	SET NOCOUNT OFF

RETURN @MovieId