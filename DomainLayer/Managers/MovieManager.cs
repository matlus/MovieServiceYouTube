using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace DomainLayer;

internal sealed class MovieManager : IDisposable
{
    private readonly ServiceLocatorBase _serviceLocator;

    private bool _disposed;

    private ConfigurationProviderBase ConfigurationProvider { get; }

    private ImdbServiceGateway? _imdbServiceGateway;

    private ImdbServiceGateway ImdbServiceGateway => _imdbServiceGateway ??= _serviceLocator.CreateImdbServiceGateway();

    private DataFacade DataFacade { get; }

    public MovieManager(ServiceLocatorBase serviceLocator)
    {
        _serviceLocator = serviceLocator;
        ConfigurationProvider = serviceLocator.CreateConfigurationProvider();
        DataFacade = new DataFacade(ConfigurationProvider.GetDbConnectionString());
    }

    public Task<int> CreateMovie(Movie movie)
    {
        ValidatorMovie.EnsureMovieIsValid(movie);
        return DataFacade.CreateMovie(movie);
    }

    public async Task CreateMovies(IEnumerable<Movie> movies)
    {
        ValidatorMovie.EnsureMoviesAreValid(movies);
        await DataFacade.CreateMovies(movies).ConfigureAwait(false);
    }

    public async Task<Movie> GetMovieById(int id)
    {
        var movie = await DataFacade.GetMovieById(id).ConfigureAwait(false);
        return movie ?? throw new MovieWithSpecifiedIdNotFoundException($"A Movie with Id: {id} was Not Found");
    }

    public Task<IEnumerable<Movie>> GetAllMovies()
    {
        var moviesTask = ImdbServiceGateway.GetAllMovies();
        var moviesFromDbTask = DataFacade.GetAllMovies();
        return GetMoviesFromCombinedTasks(moviesTask, moviesFromDbTask);
    }

    private static async Task<IEnumerable<Movie>> GetMoviesFromCombinedTasks(Task<IEnumerable<Movie>> moviesTask, Task<IEnumerable<Movie>> moviesFromDbTask)
    {
        await Task.WhenAll(moviesTask, moviesFromDbTask).ConfigureAwait(false);

        var movies = moviesTask.Result;
        var moviesFromDb = moviesFromDbTask.Result;

        var moviesList = movies.ToList();
        moviesList.AddRange(moviesFromDb);
        return moviesList;
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenre(Genre genre)
    {
        GenreParser.Validate(genre);
        var moviesFromImdbTask = ImdbServiceGateway.GetAllMovies();
        var moviesFromDbMatchingGenreTask = DataFacade.GetMovieByGenre(genre);
        await Task.WhenAll(moviesFromImdbTask, moviesFromDbMatchingGenreTask).ConfigureAwait(false);

        var moviesMatchingGenre = moviesFromDbMatchingGenreTask.Result.ToList();
        moviesMatchingGenre.AddRange(moviesFromImdbTask.Result.Where(m => m.Genre == genre));
        return moviesMatchingGenre;
    }

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (disposing && !_disposed && _imdbServiceGateway != null)
        {
            var localImdbServiceGateway = _imdbServiceGateway;
            localImdbServiceGateway.Dispose();
            _imdbServiceGateway = null;
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
