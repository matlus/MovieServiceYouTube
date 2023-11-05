using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DomainLayer;

internal sealed class MovieDataManager
{
    private readonly SqlClientFactory _sqlClientFactory = SqlClientFactory.Instance;
    private readonly string _dbConnectionString;

    public MovieDataManager(string dbConnectionString)
    {
        _dbConnectionString = dbConnectionString;
    }

    private DbConnection CreateDbConnection()
    {
        var dbConnection = _sqlClientFactory.CreateConnection();
        dbConnection.ConnectionString = _dbConnectionString;
        return dbConnection;
    }

    public async Task<int> CreateMovie(Movie movie)
    {
        var dbConnection = CreateDbConnection();
        DbTransaction? dbTransaction = null;
        DbCommand? dbCommand = null;

        try
        {
            await dbConnection.OpenAsync().ConfigureAwait(false);
            dbTransaction = dbConnection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            dbCommand = CommandFactoryMovies.CreateCommandForCreateMovie(dbConnection, dbTransaction, movie);
            await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            dbTransaction.Commit();
            return (int)dbCommand.Parameters[0].Value!;
        }
        catch (DbException e)
        {
            dbTransaction.RollbackIfNotNull();

            if (e.Message.Contains("duplicate key row in object 'dbo.Movie'", StringComparison.OrdinalIgnoreCase))
            {
                throw new DuplicateMovieException($"A Movie with the Title: {movie.Title} already exists. Please use a different title", e);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            await DisposedAsync(dbDataReader: null, dbCommand: dbCommand, dbTransaction: dbTransaction, dbConnection: dbConnection);
        }
    }

    public async Task CreateMovies(IEnumerable<Movie> movies)
    {
        var dbConnection = CreateDbConnection();
        DbTransaction? dbTransaction = null;
        DbCommand? dbCommand = null;
        try
        {
            await dbConnection.OpenAsync().ConfigureAwait(false);
            dbTransaction = dbConnection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            dbCommand = CommandFactoryMovies.CreateCommandForCreateMoviesTvpDistinctInsertInto(dbConnection, dbTransaction, movies);
            await dbCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
            dbTransaction.Commit();
        }
        catch (DbException e)
        {
            dbTransaction.RollbackIfNotNull();
            if (e.Message.Contains("duplicate key row in object 'dbo.Movie'", StringComparison.OrdinalIgnoreCase))
            {
                var movieTitles = string.Join(",", movies.Select(m => m.Title));
                throw new DuplicateMovieException($"One or more Movies with the following Titles already Exists. {movieTitles}. Please use a different title", e);
            }
            else
            {
                throw;
            }
        }
        finally
        {
            await DisposedAsync(dbDataReader: null, dbCommand: dbCommand, dbTransaction: dbTransaction, dbConnection: dbConnection);
        }
    }

    public async Task<Movie?> GetMovieById(int id)
    {
        var dbConnection = CreateDbConnection();
        DbCommand? dbCommand = null;
        DbDataReader? dbDataReader = null;
        try
        {
            await dbConnection.OpenAsync().ConfigureAwait(false);
            dbCommand = CommandFactoryMovies.CreateCommandForGetMovieById(dbConnection, id);
            dbDataReader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(false);
            return await MapToMovie(dbDataReader).ConfigureAwait(false);
        }
        finally
        {
            await DisposedAsync(dbDataReader: dbDataReader, dbCommand: dbCommand, dbTransaction: null, dbConnection: dbConnection);
        }
    }

    public async Task<IEnumerable<Movie>> GetMovieByGenre(Genre genre)
    {
        var dbConnection = CreateDbConnection();
        DbCommand? dbCommand = null;
        DbDataReader? dbDataReader = null;
        try
        {
            await dbConnection.OpenAsync().ConfigureAwait(false);
            dbCommand = CommandFactoryMovies.CreateCommandForGetMoviesByGenre(dbConnection, genre);
            dbDataReader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(false);
            return await MapToMovies(dbDataReader).ConfigureAwait(false);
        }
        finally
        {
            await DisposedAsync(dbDataReader: dbDataReader, dbCommand: dbCommand, dbTransaction: null, dbConnection: dbConnection);
        }
    }

    public async Task<IEnumerable<Movie>> GetAllMovies()
    {
        var dbConnection = CreateDbConnection();
        DbCommand? dbCommand = null;
        DbDataReader? dbDataReader = null;
        try
        {
            await dbConnection.OpenAsync().ConfigureAwait(false);
            dbCommand = CommandFactoryMovies.CreateCommandForGetAllMovies(dbConnection);
            dbDataReader = await dbCommand.ExecuteReaderAsync().ConfigureAwait(false);
            return await MapToMovies(dbDataReader).ConfigureAwait(false);
        }
        finally
        {
            await DisposedAsync(dbDataReader: dbDataReader, dbCommand: dbCommand, dbTransaction: null, dbConnection: dbConnection);
        }
    }

    private static async ValueTask DisposedAsync(DbDataReader? dbDataReader, DbCommand? dbCommand, DbTransaction? dbTransaction, DbConnection dbConnection)
    {
        await dbDataReader.DisposeIfNotNullAsync();
        await dbCommand.DisposeIfNotNullAsync();
        await dbTransaction.DisposeIfNotNullAsync();
        await dbConnection.DisposeIfNotNullAsync();
    }

    private static async Task<Movie?> MapToMovie(DbDataReader dbDataReader)
    {
        return await dbDataReader.ReadAsync().ConfigureAwait(false)
            ? new Movie(
                Title: (string)dbDataReader[0],
                Genre: GenreParser.Parse((string)dbDataReader[1]),
                Year: (int)dbDataReader[2],
                ImageUrl: (string)dbDataReader[3])
            : null;
    }

    private static async Task<IEnumerable<Movie>> MapToMovies(DbDataReader dbDataReader)
    {
        var movies = new List<Movie>();

        while (await dbDataReader.ReadAsync().ConfigureAwait(false))
        {
            movies.Add(new Movie(
                Title: (string)dbDataReader[0],
                Genre: GenreParser.Parse((string)dbDataReader[1]),
                Year: (int)dbDataReader[2],
                ImageUrl: (string)dbDataReader[3]));
        }

        return movies;
    }
}