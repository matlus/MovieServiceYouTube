using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DomainLayer;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using DomainLayer.Managers.Parsers;
using Microsoft.AspNetCore.Mvc;
using MovieServiceCore3.CustomActionResults;
using MovieServiceCore3.ResourceModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieServiceCore3.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly DomainFacade _domainFacade;

        public MoviesController(DomainFacade domainFacade)
        {
            _domainFacade = domainFacade;
        }

        // GET api/movies
        [HttpGet]
        public async Task<IEnumerable<MovieResource>> GetMovies()
        {
            var movies = await GetAllMovies();
            return MapToMovieResource(movies);
        }

        // GET api/movies/genre/drama
        [HttpGet("genre/{genreAsString}")]
        public async Task<IEnumerable<MovieResource>> GetMoviesByGenre(string genreAsString)
        {
            var genre = GenreParser.Parse(genreAsString);
            var movies = await GetMoviesByGenre(genre);
            return MapToMovieResource(movies);
        }

        // GET api/movies/id/1
        [HttpGet("id/{id}")]
        public async Task<MovieResource> GetMovie(int id)
        {
            var movie = await GetMovieById(id);
            return MapToMovieRespource(movie);
        }

        // POST api/movies
        [HttpPost]
        public async Task<ActionResult> CreateMovie([FromBody] MovieResource movieResource)
        {
            var movie = MapToMovie(movieResource);
            var newId = await CreateMovie(movie);
            return CreatedAtAction(nameof(GetMovie), new { Id = newId }, movieResource);
        }

        [ExcludeFromCodeCoverage]
        protected virtual Task<Movie> GetMovieById(int id)
        {
            return _domainFacade.GetMovieById(id);
        }

        [ExcludeFromCodeCoverage]
        protected virtual Task<IEnumerable<Movie>> GetAllMovies()
        {
            return _domainFacade.GetAllMovies();
        }

        [ExcludeFromCodeCoverage]
        protected virtual Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
        {
            return _domainFacade.GetMoviesByGenre(genre);
        }

        [ExcludeFromCodeCoverage]
        protected virtual Task<int> CreateMovie(Movie movie)
        {
            return _domainFacade.CreateMovie(movie);
        }

        private static Movie MapToMovie(MovieResource movieResource)
        {
            return new Movie(
                    title: movieResource.Title,
                    imageUrl: movieResource.ImageUrl,
                    genre: GenreParser.Parse(movieResource.Genre),
                    year: movieResource.Year);
        }

        private static MovieResource MapToMovieRespource(Movie movie)
        {
            return new MovieResource
            {
                Title = movie.Title,
                ImageUrl = movie.ImageUrl,
                Genre = GenreParser.ToString(movie.Genre),
                Year = movie.Year
            };
        }

        private static IEnumerable<MovieResource> MapToMovieResource(IEnumerable<Movie> movies)
        {
            var movieResources = new List<MovieResource>();

            foreach (var movie in movies)
            {
                movieResources.Add(MapToMovieRespource(movie));
            }

            return movieResources;
        }
    }
}
