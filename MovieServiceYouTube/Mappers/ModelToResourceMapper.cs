using DomainLayer.Managers.Models;
using DomainLayer.Managers.Parsers;
using MovieServiceCore3.ResourceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieServiceCore3.Mappers
{
    internal static class ModelToResourceMapper
    {
        public static Movie MapToMovie(MovieResource movieResource)
        {
            return new Movie(
                    title: movieResource.Title,
                    imageUrl: movieResource.ImageUrl,
                    genre: GenreParser.Parse(movieResource.Genre),
                    year: movieResource.Year);
        }

        public static MovieResource MapToMovieResource(Movie movie)
        {
            return new MovieResource
            {
                Title = movie.Title,
                ImageUrl = movie.ImageUrl,
                Genre = GenreParser.ToString(movie.Genre),
                Year = movie.Year
            };
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
}
