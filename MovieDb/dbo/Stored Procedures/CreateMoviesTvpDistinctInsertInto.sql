CREATE PROCEDURE [dbo].[CreateMoviesTvpDistinctInsertInto]
	@MovieTvp MovieTvp READONLY
AS
	SET XACT_ABORT ON
	SET NOCOUNT ON	

	-- Insert Distinct Genres into the Genre table
	INSERT
	INTO	dbo.Genre
	SELECT DISTINCT(Genre)
	FROM	@MovieTvp
	EXCEPT
	SELECT	dbo.Genre.Title
	FROM	dbo.Genre

	-- Insert Movies into the Movie Table
	INSERT
	INTO	dbo.Movie (Title, [Year], ImageUrl)
	SELECT	Title, [Year], ImageUrl
	FROM	@MovieTvp m
	
	-- Select the Movie.Id and the Genre.Id columns and insert into the Assoc_MovieGenre
	INSERT
	INTO	dbo.Assoc_MovieGenre
	SELECT	dbo.Movie.Id, dbo.Genre.Id
	FROM	@MovieTvp mtvp
	INNER
	JOIN	dbo.Genre
	ON		dbo.Genre.Title = mtvp.Genre
	INNER
	JOIN	dbo.Movie
	ON		dbo.Movie.Title = mtvp.Title

	SET NOCOUNT OFF
RETURN 0