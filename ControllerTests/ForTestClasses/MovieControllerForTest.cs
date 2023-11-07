using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainLayer;
using MovieServiceCore3.Controller;

namespace ControllerTests.ForTestClasses;

internal sealed class MovieControllerForTest : MoviesController
{
    private readonly IEnumerable<Movie> _movies;
    private readonly Movie _movie;
    private readonly Exception _exception;

    public MovieControllerForTest(DomainFacade domainFacade)
        : base(domainFacade)
    {
    }

    public MovieControllerForTest(IEnumerable<Movie> movies)
        : base(null) => _movies = movies;

    public MovieControllerForTest(Movie movie)
 : base(null) => _movie = movie;

    public MovieControllerForTest(Exception exception)
        : base(null) => _exception = exception;

    protected override Task<IEnumerable<Movie>> GetAllMovies()
    {
        return _exception != null ? throw _exception : Task.FromResult(_movies);
    }

    protected override Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
    {
        return Task.FromResult(_movies);
    }

    protected override Task<Movie> GetMovieById(int id)
    {
        return Task.FromResult(_movie);
    }

    protected override Task<int> CreateMovie(Movie movie)
    {
        return _exception != null ? throw _exception : Task.FromResult(0);
    }
}
