using AcceptanceTests.DomainLayer.Managers.ServiceLocators;
using DomainLayer.Managers.DataLayer.DataManagers;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Threading.Tasks;

namespace ClassTests.Managers.DataLayer.DataManagers
{
    [TestClass]
    public class MovieDataManagerTests
    {
        private readonly string _dbConnectionString;
        public MovieDataManagerTests()
        {
            _dbConnectionString = new ServiceLocatorForAcceptanceTesting(null).CreateConfigurationProvider().GetDbConnectionString();
        }

        /// <summary>
        /// It is impossible to get past the business layer with an invalid Movie, as a result
        /// It's impossible to have the database throw an exception when creating a movie other than
        /// an index related issue (which we're already able to test). This edge case test attempts
        /// to create a movie record with an invalid movie so as to force the database to throw another
        /// exception
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        [TestCategory("Edgecase Class Test")]
        public async Task CreateMovie_WhenAMovieDoesNotHaveRequiredData_Throws()
        {
            var movieDataManager = new MovieDataManager(_dbConnectionString);
            var movie = new Movie(null, "http://www.somedomain.com/movies/img/23", Genre.Action, 2019);

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
    }
}
