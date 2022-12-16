using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ControllerTests.ForTestClasses;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Exceptions;
using DomainLayer.Managers.Models;
using DomainLayer.Managers.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieServiceCore3.ResourceModels;
using Testing.Shared;

namespace ControllerTests
{
    [TestClass]
    public class MovieControllerTests
    {
        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMovies_WhenThereAreNoExceptions_ReturnsAllMovies()
        {
            // Arrange
            var expectedMovies = RandomMovieGenerator.GenerateRandomMovies(50);
            var moviesController = new MovieControllerForTest(expectedMovies);

            // Act
            var actualMovieResources = await moviesController.GetMovies();

            // Assert
            var actualMovies = MapToMvoies(actualMovieResources);
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMovies_WhenAnExceptionIsThrownInTheDomainLayer_ShouldThrow()
        {
            // Arrange
            var expectedMessage = "Some Exception Message";
            var expectedException = new ConfigurationSettingMissingException(expectedMessage);            
            var moviesController = new MovieControllerForTest(expectedException);

            // Act
            try
            {
                await moviesController.GetMovies();
                Assert.Fail("We were expecting an exception of type: ConfigurationSettingMissingException to be thrown, but no exception was thrown");
            }
            catch (ConfigurationSettingMissingException e)
            {
                // Assert
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithAValidGenre_ReturnsMoviesByGenre()
        {
            // Arrange
            //// It is not the controller's responsibility to filter by genre
            ///  or ensure all movies being returned have the genre specified
            var expectedMovies = RandomMovieGenerator.GenerateRandomMovies(50);
            var moviesController = new MovieControllerForTest(expectedMovies);
            var validGenreAsString = GenreParser.ToString(Genre.SciFi);

            // Act
            var actualMovieResources = await moviesController.GetMoviesByGenre(validGenreAsString);

            // Assert
            var actualMovies = MapToMvoies(actualMovieResources);
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithAInValidGenre_ShouldThrow()
        {
            // Arrange
            var moviesController = new MovieControllerForTest(movies: null);
            var invalidGenreAsString = "xxxxx";

            // Act
            try
            {
                await moviesController.GetMoviesByGenre(invalidGenreAsString);
                Assert.Fail("We were expecting an exception of type InvalidGenreException to be thrown, but no exception was thrown");
            }
            catch (InvalidGenreException e)
            {
                // Assert
                StringAssert.Contains(e.Message, invalidGenreAsString);
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithAnEmptyGenre_ShouldThrow()
        {
            // Arrange
            var moviesController = new MovieControllerForTest(movies: null);
            var emptyGenreAsString = string.Empty;

            // Act
            try
            {
                await moviesController.GetMoviesByGenre(emptyGenreAsString);
                Assert.Fail("We were expecting an exception of type InvalidGenreException to be thrown, but no exception was thrown");
            }
            catch (InvalidGenreException e)
            {
                // Assert
                StringAssert.Contains(e.Message, "The string can not be null or empty");
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithANullGenre_ShouldThrow()
        {
            // Arrange
            var moviesController = new MovieControllerForTest(movies: null);
            string nullGenreAsString = null;

            // Act
            try
            {
                await moviesController.GetMoviesByGenre(nullGenreAsString);
                Assert.Fail("We were expecting an exception of type InvalidGenreException to be thrown, but no exception was thrown");
            }
            catch (InvalidGenreException e)
            {
                // Assert
                StringAssert.Contains(e.Message, "The string can not be null or empty");
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task GetMovieById_WhenThereAreNoExceptions_ReturnsAllMovies()
        {
            // Arrange
            var expectedMovies = RandomMovieGenerator.GenerateRandomMovies(1);
            var moviesController = new MovieControllerForTest(expectedMovies.Single());
            var irrelevantMovieId = -1; //// We simply want to make sure the controler returns what was given to it by the Domain

            // Act
            var actualMovieResources = await moviesController.GetMovie(irrelevantMovieId);

            // Assert
            var actualMovies = MapToMvoies(new[] { actualMovieResources });
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task CreateMovie_WhenProvidedWithAValidMovieResource_Succeeds()
        {
            // Arrange
            var movieResource = new MovieResource
            {
                Title = "Some Title",
                Genre = GenreParser.ToString(Genre.Action),
                ImageUrl = "Some Url",
                Year = 1900
            };

            var moviesController = new MovieControllerForTest(movies: null);            

            // Act
            await moviesController.CreateMovie(movieResource);

            // Assert
            // Nothing to Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public async Task CreateMovie_WhenAnExceptionIsThrownInTheDomainLayer_ShouldThrow()
        {
            // Arrange
            var expectedMessage = "Some Exception Message";
            var expectedException = new DuplicateMovieException(expectedMessage);
            var moviesController = new MovieControllerForTest(expectedException);
            var movieResource = new MovieResource
            {
                Title = "Some Title",
                Genre = GenreParser.ToString(Genre.Action),
                ImageUrl = "Some Url",
                Year = 1900
            };

            // Act
            try
            {
                await moviesController.CreateMovie(movieResource);
                Assert.Fail("We were expecting an exception of type: DuplicateMovieException to be thrown, but no exception was thrown");
            }
            catch (DuplicateMovieException e)
            {
                // Assert
                Assert.AreEqual(expectedMessage, e.Message);
            }
        }


        private static IEnumerable<Movie> MapToMvoies(IEnumerable<MovieResource> movieResources)
        {
            var movies = new List<Movie>();

            foreach (var movieResource in movieResources)
            {
                movies.Add(
                    new Movie(
                        movieResource.Title,
                        movieResource.ImageUrl,
                        GenreParser.Parse(movieResource.Genre),
                        movieResource.Year)
                );
            }

            return movies;
        }
    }
}
