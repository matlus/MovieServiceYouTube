using System.Collections.Generic;
using DomainLayer;
using MovieServiceCore3.ResourceModels;

namespace MovieServiceCore3.Mappers;

internal static class ModelToResourceMapper
{
    public static Movie MapToMovie(MovieResource movieResource)
    {
        return new Movie(
                movieResource.Title,
                movieResource.ImageUrl,
                GenreParser.Parse(movieResource.Genre),
                movieResource.Year);
    }

    public static MovieResource MapToMovieResource(Movie movie)
    {
        return new MovieResource(
            Title: movie.Title,
            ImageUrl: movie.ImageUrl,
            Genre: GenreParser.ToString(movie.Genre),
            Year: movie.Year);
    }

    public static IEnumerable<MovieResource> MapToMovieResource(IEnumerable<Movie> movies)
    {
        var movieResources = new List<MovieResource>();

        foreach (var movie in movies)
        {
            movieResources.Add(MapToMovieResource(movie));
        }

        return movieResources;
    }
}
