using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcceptanceTests.DomainLayer.Managers.ServiceLocators;
using AcceptanceTests.ModelBuilder;
using AcceptanceTests.TestDataGenerators;
using AcceptanceTests.TestMediators;
using DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Shared;
using Testing.Shared.TestingHelpers;

namespace AcceptanceTests;

[TestClass]
public class DomainFacadeTests
{
    private readonly IEnumerable<Movie> moviesInDb;
    private readonly string dbConnectionString;
    private readonly ModelBuilder<Movie> movieModelBuilder;

    public DomainFacadeTests()
    {
        dbConnectionString = new ServiceLocatorForAcceptanceTesting(null!).CreateConfigurationProvider().GetDbConnectionString();
        moviesInDb = MovieTestDataGenerator.GetAllMovies(dbConnectionString).GetAwaiter().GetResult();
        movieModelBuilder = InitializeMovieModelBuilder();
    }

    private static ModelBuilder<Movie> InitializeMovieModelBuilder()
    {
        return new ModelBuilder<Movie>()
          .For(m => m.Title, () => RandomStringGenerator.GetRandomAciiString(50))
          .For(m => m.ImageUrl, () => $"https://www.{RandomStringGenerator.GetRandomAciiString(20)}.com")
          .For(m => m.Genre, () => (Genre)RandomStringGenerator.GetRandomInt(0, 4))
          .For(m => m.Year, () => RandomStringGenerator.GetRandomInt(1900, DateTime.Today.Year));
    }

    private static (DomainFacade domainFacade, TestMediator testMediator) CreateDomainFacade()
    {
        var testMediator = new TestMediator();
        var serviceLocatorForAcceptanceTesting = new ServiceLocatorForAcceptanceTesting(testMediator);
        return (new DomainFacade(serviceLocatorForAcceptanceTesting), testMediator);
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetAllMovies_WhenCalled_ReturnsAllMovies()
    {
        // Arrange 
        var (domainFacade, testMediator) = CreateDomainFacade();

        try
        {
            var moviesFromService = RandomMovieGenerator.GenerateRandomMovies(50);
            testMediator.MoviesForGetAllMovies = moviesFromService;

            var expectedMovies = new List<Movie>();
            expectedMovies.AddRange(moviesFromService);
            expectedMovies.AddRange(moviesInDb);

            // Act
            var actualMovies = await domainFacade.GetAllMovies();

            // Assert
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetMoviesByGenre_WhenCalledWithValidGenre_ShouldReturnMoviesFilteredByGenre()
    {
        // Arrange
        var (domainFacade, testMediator) = CreateDomainFacade();

        try
        {
            var randomMovies = new List<Movie>();

            var moviesFromService = RandomMovieGenerator.GenerateRandomMovies(50);
            testMediator.MoviesForGetAllMovies = moviesFromService;

            randomMovies.AddRange(moviesFromService);
            randomMovies.AddRange(moviesInDb);
            var expectGenre = Genre.Comedy;
            var expectedMovies = randomMovies.Where(m => m.Genre == expectGenre).ToList();

            // Act
            var actualMovies = await domainFacade.GetMoviesByGenre(expectGenre);

            // Assert
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetMoviesById_WhenCalledWithValidId_ShouldReturnMovieMatchingId()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();

        try
        {
            var expectedMovie = moviesInDb.First();
            var expectedMovieId = await MovieTestDataGenerator.GetMovieIdByTitle(dbConnectionString, expectedMovie.Title);

            // Act
            var actualMovie = await domainFacade.GetMovieById(expectedMovieId);

            // Assert
            MovieAssertions.AssertMoviesAreEqual(new[] { expectedMovie }, new[] { actualMovie });
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetMoviesById_WhenCalledWithANonExistantMovieId_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var nonExistantMovieId = -1;

        try
        {
            // Act
            _ = await domainFacade.GetMovieById(nonExistantMovieId);
            Assert.Fail("We Were Expecting a MovieWithSpecifiedIdNotFoundException to be thrown, but no exception was thrown.");
        }
        catch (MovieWithSpecifiedIdNotFoundException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"Id: {nonExistantMovieId}");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetMoviesByGenre_WhenCalledWithAnInvalidGenre_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidGenre = (Genre)1000;

        try
        {
            // Act
            await domainFacade.GetMoviesByGenre(invalidGenre);
            Assert.Fail("We were expecting an InvalidGenreException expection to be thrown but no exception was thrown");
        }
        catch (InvalidGenreException e)
        {
            StringAssert.Contains(e.Message, $"{(int)invalidGenre} is not a valid Genre");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetAllMovies_ServiceEndpointIsNotFound_ShouldThrow()
    {
        // Arrange
        var (domainFacade, testMediator) = CreateDomainFacade();

        try
        {
            testMediator.ExceptionInformation
                = new ExceptionInformation { ExceptionReason = ExceptionReason.NotFond };

            // Act
            await domainFacade.GetAllMovies();
            Assert.Fail("We were expecting an exception of type ImdbServiceNotFoundException to be thrown, but no exception was thrown");
        }
        catch (ImdbServiceNotFoundException e)
        {
            // Assert
            AssertEx.EnsureExceptionMessageContains(e, "resulted in a Not Found Status Code");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetAllMovies_WhenProxyAuthenticationFailed_ShouldThrow()
    {
        // Arrange
        var (domainFacade, testMediator) = CreateDomainFacade();

        try
        {
            testMediator.ExceptionInformation
                = new ExceptionInformation { ExceptionReason = ExceptionReason.ProxyAuthenticationRequired };

            // Act
            await domainFacade.GetAllMovies();
            Assert.Fail("We were expecting an exception of type ImdbProxyAuthenticationRequiredException to be thrown, but no exception was thrown");
        }
        catch (ImdbProxyAuthenticationRequiredException e)
        {
            // Assert
            AssertEx.EnsureExceptionMessageContains(e, "Imdb Service", "status code", "ProxyAuthenticationRequired");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task GetAllMovies_WhenServiceIsNotAvailable_ShouldThrow()
    {
        // Arrange
        var (domainFacade, testMediator) = CreateDomainFacade();

        try
        {
            testMediator.ExceptionInformation
                = new ExceptionInformation { ExceptionReason = ExceptionReason.ServiceUnavailable };

            // Act
            await domainFacade.GetAllMovies();
            Assert.Fail("We were expecting an exception of type ImdbServiceNotFoundException to be thrown, but no exception was thrown");
        }
        catch (ImdbServiceNotFoundException e)
        {
            // Assert
            AssertEx.EnsureExceptionMessageContains(e, "Imdb Service", "status code", "ServiceUnavailable");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenCalledWithAValidMovieNonExistent_Succeed()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var expectedMovie = movieModelBuilder.Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(expectedMovie);

            // Assert
            var actualMovie = await MovieTestDataGenerator.RetrieveMovie(dbConnectionString, expectedMovie.Title);
            MovieAssertions.AssertMoviesAreEqual(new[] { expectedMovie }, new[] { actualMovie! });
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenDuplicateMovieIsCreated_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var expectedMovie = movieModelBuilder.Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(expectedMovie);
            await domainFacade.CreateMovie(expectedMovie);
            Assert.Fail("We were expecting a DuplicateMovieException exception to be thrown, but no exception was thrown");
        }
        catch (DuplicateMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"Title: {expectedMovie.Title} already exists");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieIsNull_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        Movie nullMovie = default!;

        try
        {
            // Act
            await domainFacade.CreateMovie(nullMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            AssertEx.EnsureExceptionMessageContains(e, "movie", "parameter can not be null.");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieTitleIsNull_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Title, null)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid Title and can not be null");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieTitleIsEmpty_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Title, string.Empty)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid Title and can not be Empty");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieTitleIsWhitespaces_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Title, "     ")
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid Title and can not be Whitespaces");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "ImageUrl", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieImageUrlIsNull_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.ImageUrl, null)
            .Build();
        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid ImageUrl and can not be null");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieImageUrlIsEmpty_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.ImageUrl, string.Empty)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid ImageUrl and can not be Empty");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieImageUrlIsWhitespaces_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.ImageUrl, "     ")
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, " valid ImageUrl and can not be Whitespaces");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "Genre", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieYearIsLessThan1900_ShouldThrow()
    {
        // Arrange
        var minimumYear = 1900;
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Year, minimumYear - 1)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"The Year, must be between {minimumYear} and {DateTime.Today.Year}");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Genre");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieYearIsGreaterThanTodaysYear_ShouldThrow()
    {
        // Arrange
        var todaysYear = DateTime.Today.Year;
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Year, todaysYear + 1)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"The Year, must be between 1900 and {todaysYear}");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Genre");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovie_WhenMovieGenreIsNotValid_ShouldThrow()
    {
        // Arrange
        var invalidGenre = (Genre)1000;
        var (domainFacade, _) = CreateDomainFacade();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Genre, invalidGenre)
            .Build();

        try
        {
            // Act
            await domainFacade.CreateMovie(invalidMovie);
            Assert.Fail("We were expecting a InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"The Genre: {invalidGenre} is not a valid Genre");
            AssertEx.EnsureExceptionMessageDoesNotContains(e, "Title", "ImageUrl", "Year");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovies_WhenCalledWithValidMoviesNonExistent_Succeed()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var expectedMovies = RandomMovieGenerator.GenerateRandomMovies(5);
        try
        {
            // Act
            await domainFacade.CreateMovies(expectedMovies);

            // Assert
            var actualMovies = await MovieTestDataGenerator.RetrieveMovies(dbConnectionString, expectedMovies.Select(m => m.Title));
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovies_WhenCalledWithPreExistingMovies_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var expectedMovies = RandomMovieGenerator.GenerateRandomMovies(2);
        try
        {
            // Act
            await domainFacade.CreateMovies(expectedMovies);
            await domainFacade.CreateMovies(expectedMovies);
            Assert.Fail("We were expecting a DuplicateMovieException exception to be thrown, but no exception was thrown");
        }
        catch (DuplicateMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, $"One or more Movies with the following Titles already Exists");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovies_WhenCalledWithEmptyCollection_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var emptyMovies = Enumerable.Empty<Movie>();
        try
        {
            // Act
            await domainFacade.CreateMovies(emptyMovies);
            Assert.Fail("We were expecting an InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, "The movies Collection Must Contain One or More Movies and Can't be Empty");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }

    [TestMethod]
    [TestCategory("Acceptance Test")]
    public async Task CreateMovies_WhenCalledWithOneOrMoreInvalidMovies_ShouldThrow()
    {
        // Arrange
        var (domainFacade, _) = CreateDomainFacade();
        var movies = RandomMovieGenerator.GenerateRandomMovies(2).ToList();
        var invalidMovie = movieModelBuilder
            .Set(m => m.Title, null)
            .Build();
        
        movies.Add(invalidMovie);

        try
        {
            // Act
            await domainFacade.CreateMovies(movies);
            Assert.Fail("We were expecting an InvalidMovieException exception to be thrown, but no exception was thrown");
        }
        catch (InvalidMovieException e)
        {
            // Assert
            StringAssert.Contains(e.Message, "The Movie Title must be a valid Title and can not be null");
        }
        finally
        {
            domainFacade.Dispose();
        }
    }
}
