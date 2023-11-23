using System.Data.Common;
using System.Threading.Tasks;
using AcceptanceTests.DomainLayer.Managers.ServiceLocators;
using DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassTests.Managers.DataLayer.DataManagers;

[TestClass]
public class MovieDataManagerTests
{
    private readonly string _dbConnectionString;

    public MovieDataManagerTests() => _dbConnectionString = new ServiceLocatorForAcceptanceTesting(null!).CreateConfigurationProvider().GetDbConnectionString();

    /// <summary>
    /// It is impossible to get past the business layer with an invalid Movie, as a result
    /// It's impossible to have the database throw an exception when creating a movie other than
    /// an index related issue (which we're already able to test). This edge case test attempts
    /// to create a movie record with an invalid movie so as to force the database to throw another
    /// exception.
    /// </summary>
    /// <returns>Task.</returns>
    [TestMethod]
    [TestCategory("Edgecase Class Test")]
    public async Task CreateMovie_WhenAMovieDoesNotHaveRequiredData_Throws()
    {
        var movieDataManager = new MovieDataManager(_dbConnectionString);
        var movie = new Movie(null!, "http://www.somedomain.com/movies/img/23", Genre.Action, 2019);

        // Act
        try
        {
            await movieDataManager.CreateMovie(movie);
            Assert.Fail("We were expecting a DbException to be thrown, but no exception was thrown");
        }
        catch (DbException e)
        {
            StringAssert.Contains(e.Message, "'CreateMovie' expects parameter '@Title', which was not supplied");
        }
    }

    /// <summary>
    /// It is impossible to get past the business layer with invalid Movies, as a result
    /// It's impossible to have the database throw an exception when creating a movie other than
    /// an index related issue (which we're already able to test). This edge case test attempts
    /// to create movies with one movie that is not valid so as to force the database to throw another
    /// exception.
    /// </summary>
    /// <returns>Task.</returns>
    [TestMethod]
    [TestCategory("Edgecase Class Test")]
    public async Task CreateMovies_WhenOneOfTheMoviesDoesNotHaveRequiredData_Throws()
    {
        var movieDataManager = new MovieDataManager(_dbConnectionString);
        var movies = new[] { new Movie(null!, "http://www.somedomain.com/movies/img/23", Genre.Action, 2019) };

        // Act
        try
        {
            await movieDataManager.CreateMovies(movies);
            Assert.Fail("We were expecting a DbException to be thrown, but no exception was thrown");
        }
        catch (DbException e)
        {
            StringAssert.Contains(e.Message, "Cannot insert the value NULL into column 'Title', table '@MovieTvp'");
        }
    }
}
