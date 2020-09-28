CREATE PROCEDURE [dbo].[CreateMoviesTvpMergeInsertInto]
	@MovieTvp MovieTvp READONLY
AS
	SET XACT_ABORT ON
	SET NOCOUNT ON

	MERGE
	INTO	dbo.Genre as g 
	USING	(SELECT DISTINCT(Genre)
			FROM @MovieTvp) as mtvp 
			ON g.Title = mtvp.Genre 
	WHEN NOT MATCHED THEN 
	INSERT
	VALUES	(mtvp.Genre);


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