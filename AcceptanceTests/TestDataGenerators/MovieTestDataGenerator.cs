using DomainLayer.Managers.Models;
using DomainLayer.Managers.Parsers;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AcceptanceTests.TestDataGenerators
{
    internal static class MovieTestDataGenerator
    {
        private static readonly SqlClientFactory sqlClientFactory = SqlClientFactory.Instance;

        static DbConnection CreateDbConnection(string dbConnectionString)
        {
            var dbConnection = sqlClientFactory.CreateConnection();
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
            DbConnection dbConnection = null;
            DbCommand dbCommand = null;
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
                dbCommand?.Dispose();
                dbConnection.Dispose();
            }
        }

        private static IEnumerable<Movie> MapToMovies(DbDataReader dbDataReader)
        {
            var movies = new List<Movie>();

            while (dbDataReader.Read())
            {
                movies.Add(new Movie
                (
                    title: (string)dbDataReader[0],
                    genre: GenreParser.Parse((string)dbDataReader[1]),
                    year: (int)dbDataReader[2],
                    imageUrl: (string)dbDataReader[3]
                ));
            }

            return movies;
        }

        public static async Task<Movie> RetrieveMovie(string dbConnectionString, string title)
        {
            DbConnection dbConnection = null;            
            DbCommand dbCommand = null;
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

                    return new Movie
                    (
                        title: (string)dbDataReader[MovieTitleIndex],
                        imageUrl: (string)dbDataReader[MovieImageUrlIndex],
                        genre: GenreParser.Parse((string)dbDataReader[MovieGenreIndex]),
                        year: (int)dbDataReader[MovieYearIndex]
                    );
                }

                return null;
            }
            finally
            {
                dbCommand?.Dispose();
                dbConnection.Dispose();
            }
        }
    }
}
