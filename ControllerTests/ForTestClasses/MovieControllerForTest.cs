using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DomainLayer;
using MovieServiceCore3.Controller;

namespace ControllerTests.ForTestClasses;

internal sealed class MovieControllerForTest : MoviesController
{
    private readonly IEnumerable<Movie> _movies = default!;
    private readonly Movie _movie = default!;
    private readonly Exception _exception = default!;

    public MovieControllerForTest(IEnumerable<Movie> movies)
        : base(default!) => _movies = movies;

    public MovieControllerForTest(Movie movie)
        : base(default!) => _movie = movie;

    public MovieControllerForTest(Exception exception)
        : base(default!) => _exception = exception;

    protected override Task<ImmutableList<Movie>> GetAllMovies()
    {
        return _exception != null ? throw _exception : Task.FromResult(_movies.ToImmutableList());
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