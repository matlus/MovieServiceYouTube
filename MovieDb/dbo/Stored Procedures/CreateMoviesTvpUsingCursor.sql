CREATE PROCEDURE [dbo].[CreateMoviesTvpUsingCursor]
	@MovieTvp MovieTvp READONLY
AS
	SET XACT_ABORT ON
	SET NOCOUNT ON

	DECLARE @title varchar(50), @genre varchar(50), @year int, @imageurl varchar(200)

	DECLARE movie_cursor CURSOR FOR   
	SELECT Title, Genre, [Year], ImageUrl
	FROM @MovieTvp
  
	OPEN movie_cursor  

	FETCH NEXT FROM movie_cursor   
	INTO @title, @genre, @year, @imageUrl
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN
		EXEC dbo.CreateMovie @title, @genre, @year, @imageUrl
		FETCH NEXT FROM movie_cursor
		INTO @title, @genre, @year, @imageUrl
	END
	CLOSE movie_cursor;  
	DEALLOCATE movie_cursor;  

	SET NOCOUNT OFF
RETURN 0