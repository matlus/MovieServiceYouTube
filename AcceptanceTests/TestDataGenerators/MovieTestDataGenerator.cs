using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DomainLayer;

namespace AcceptanceTests.TestDataGenerators;

internal static class MovieTestDataGenerator
{
    private static readonly SqlClientFactory SqlClientFactory = SqlClientFactory.Instance;

    private static DbConnection CreateDbConnection(string dbConnectionString)
    {
        var dbConnection = SqlClientFactory.CreateConnection();
        dbConnection.ConnectionString = dbConnectionString;
        return dbConnection;
    }

    private static void AddDbParameter(DbCommand dbCommand, string parameterName, object value, DbType dbType, int size)
    {
        var dbParameter = dbCommand.CreateParameter();
        dbParameter.ParameterName = parameterName;
        dbParameter.Value = value;
        dbParameter.DbType = dbType;
        dbParameter.Size = size;
        dbCommand.Parameters.Add(dbParameter);
    }

    public static async Task<IEnumerable<Movie>> GetAllMovies(string dbConnectionString)
    {
        DbConnection dbConnection = default!;
        DbCommand dbCommand = default!;
        try
        {
            dbConnection = CreateDbConnection(dbConnectionString);
            await dbConnection.OpenAsync();
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.StoredProcedure;
            dbCommand.CommandText = "dbo.GetAllMovies";
            var dbDataReader = await dbCommand.ExecuteReaderAsync();
            return MapToMovies(dbDataReader);
        }
        finally
        {
            await DisposeAsync(dbDataReader: null, dbCommand, dbTransaction: null, dbConnection);
        }
    }

    public static async Task<int> GetMovieIdByTitle(string dbConnectionString, string title)
    {
        DbConnection dbConnection = default!;
        DbCommand dbCommand = default!;
        try
        {
            dbConnection = CreateDbConnection(dbConnectionString);
            await dbConnection.OpenAsync();
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = $"SELECT Id FROM dbo.MovieVw WHERE Title = '{title}'";
            var movieId = (int)await dbCommand.ExecuteScalarAsync()!;
            return movieId;
        }
        finally
        {
            await DisposeAsync(dbDataReader: null, dbCommand, dbTransaction: null, dbConnection);
        }
    }

    private static ImmutableList<Movie> MapToMovies(DbDataReader dbDataReader)
    {
        var movies = ImmutableList.Create<Movie>();

        while (dbDataReader.Read())
        {
            movies = movies.Add(new Movie(
                Title: (string)dbDataReader[0],
                Genre: GenreParser.Parse((string)dbDataReader[1]),
                Year: (int)dbDataReader[2],
                ImageUrl: (string)dbDataReader[3]));
        }

        return movies;
    }

    public static async Task<Movie?> RetrieveMovie(string dbConnectionString, string title)
    {
        DbConnection dbConnection = default!;
        DbCommand dbCommand = default!;
        try
        {
            dbConnection = CreateDbConnection(dbConnectionString);
            await dbConnection.OpenAsync();
            dbCommand = dbConnection.CreateCommand();

            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = "SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw WHERE Title = @Title";

            AddDbParameter(dbCommand, "@Title", title, DbType.String, 50);
            var dbDataReader = await dbCommand.ExecuteReaderAsync();
            if (dbDataReader.Read())
            {
                const int MovieTitleIndex = 0;
                const int MovieGenreIndex = 1;
                const int MovieYearIndex = 2;
                const int MovieImageUrlIndex = 3;

                return new Movie(
                    Title: (string)dbDataReader[MovieTitleIndex],
                    ImageUrl: (string)dbDataReader[MovieImageUrlIndex],
                    Genre: GenreParser.Parse((string)dbDataReader[MovieGenreIndex]),
                    Year: (int)dbDataReader[MovieYearIndex]);
            }

            return null;
        }
        finally
        {
            await DisposeAsync(dbDataReader: null, dbCommand, dbTransaction: null, dbConnection);
        }
    }

    public static async Task<IEnumerable<Movie>> RetrieveMovies(string dbConnectionString, IEnumerable<string> titles)
    {
        DbConnection dbConnection = default!;
        DbCommand dbCommand = default!;
        try
        {
            dbConnection = CreateDbConnection(dbConnectionString);
            await dbConnection.OpenAsync();
            dbCommand = dbConnection.CreateCommand();

            dbCommand.CommandType = CommandType.Text;

            var commandText = "SELECT	Title, Genre, Year, ImageUrl FROM dbo.MovieVw WHERE Title IN (";

            foreach (var title in titles)
            {
                commandText += $"'{title}', ";
            }

            commandText = commandText[..^2];

            commandText += ")";
            dbCommand.CommandText = commandText;

            var dbDataReader = await dbCommand.ExecuteReaderAsync();

            const int MovieTitleIndex = 0;
            const int MovieGenreIndex = 1;
            const int MovieYearIndex = 2;
            const int MovieImageUrlIndex = 3;

            var movies = new List<Movie>();
            while (dbDataReader.Read())
            {
                movies.Add(new Movie(
                    Title: (string)dbDataReader[MovieTitleIndex],
                    ImageUrl: (string)dbDataReader[MovieImageUrlIndex],
                    Genre: GenreParser.Parse((string)dbDataReader[MovieGenreIndex]),
                    Year: (int)dbDataReader[MovieYearIndex]));
            }

            return movies;
        }
        finally
        {
            await DisposeAsync(dbDataReader: null, dbCommand, dbTransaction: null, dbConnection);
        }
    }

    private static async ValueTask DisposeAsync(DbDataReader? dbDataReader, DbCommand? dbCommand, DbTransaction? dbTransaction, DbConnection dbConnection)
    {
        await dbDataReader.DisposeIfNotNullAsync();
        await dbCommand.DisposeIfNotNullAsync();
        await dbTransaction.DisposeIfNotNullAsync();
        await dbConnection.DisposeIfNotNullAsync();
    }
}
