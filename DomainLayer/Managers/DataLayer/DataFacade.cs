using System.Collections.Generic;
using System.Threading.Tasks;

namespace DomainLayer;

internal sealed class DataFacade
{
    private MovieDataManager MovieDataManager { get; }

    public DataFacade(string dbConnectionString)
    {
        MovieDataManager = new MovieDataManager(dbConnectionString);
    }

    public Task<int> CreateMovie(Movie movie)
    {
        return MovieDataManager.CreateMovie(movie);
    }

    public async Task CreateMovies(IEnumerable<Movie> movies)
    {
        await MovieDataManager.CreateMovies(movies).ConfigureAwait(false);
    }

    public Task<Movie?> GetMovieById(int id)
    {
        return MovieDataManager.GetMovieById(id);
    }

    public Task<IEnumerable<Movie>> GetMovieByGenre(Genre genre)
    {
        return MovieDataManager.GetMovieByGenre(genre);
    }

    public Task<IEnumerable<Movie>> GetAllMovies()
    {
        return MovieDataManager.GetAllMovies();
    }
}
