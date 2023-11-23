using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AcceptanceTests")]
[assembly: InternalsVisibleTo("Testing.Shared")]
[assembly: InternalsVisibleTo("ClassTests")]

namespace DomainLayer;

public sealed class DomainFacade : IDisposable
{
    private bool _disposed;

    private MovieManager MovieManager { get; }

    public DomainFacade()
      : this(new ServiceLocator())
    {
    }

    internal DomainFacade(ServiceLocatorBase serviceLocator) => MovieManager = new MovieManager(serviceLocator);

    public Task<Movie> GetMovieById(int id)
    {
        return MovieManager.GetMovieById(id);
    }

    public Task<IEnumerable<Movie>> GetAllMovies()
    {
        return MovieManager.GetAllMovies();
    }

    public Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
    {
        return MovieManager.GetMoviesByGenre(genre);
    }

    public Task<int> CreateMovie(Movie movie)
    {
        return MovieManager.CreateMovie(movie);
    }

    public Task CreateMovies(IEnumerable<Movie> movies)
    {
        return MovieManager.CreateMovies(movies);
    }

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (disposing && !_disposed && MovieManager != null)
        {
            MovieManager.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
