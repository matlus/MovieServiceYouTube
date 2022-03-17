using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Exceptions;
using DomainLayer.Managers.Parsers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieServiceCore3;
using MovieServiceCore3.ResourceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Testing.Shared;

namespace EndToEndIntegrationTests
{
    [TestClass]
    public class EndToEndIntegrationTests
    {
        private static HttpClient _httpClient;

        public EndToEndIntegrationTests()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateClient();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _httpClient.Dispose();
        }

        [TestMethod]
        [TestCategory("EndToEndIntegration Test")]
        public async Task GetMovies_WhenOperatingNormally_ShouldSucceed()
        {
            // Act
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var httpResponseMessage = await _httpClient.GetAsync("api/movies/");

            // Assert
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {                
                var actualMovieResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<MovieResource>>();
                Assert.IsTrue(actualMovieResource.Any());
            }
            else
            {
                var httpContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Assert.Fail($"Reason Phrase: {httpResponseMessage.ReasonPhrase} with Content: {httpContent}");
            }
        }

        [TestMethod]
        [TestCategory("EndToEndIntegration Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithAValidAndExistingGenre_ShouldReturnMoviesForGenre()
        {
            // Act
            var validGenre = GenreParser.ToString(Genre.SciFi);
            var httpResponseMessage = await _httpClient.GetAsync($"api/movies/genre/{validGenre}");

            // Assert
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                var actualMovieResource = await httpResponseMessage.Content.ReadAsAsync<IEnumerable<MovieResource>>();
                Assert.IsTrue(actualMovieResource.All(m => m.Genre == validGenre));
            }
            else
            {
                var httpContent = await httpResponseMessage.Content.ReadAsStringAsync();
                Assert.Fail($"Reason Phrase: {httpResponseMessage.ReasonPhrase} with Content: {httpContent}");
            }
        }

        [TestMethod]
        [TestCategory("EndToEndIntegration Test")]
        public async Task GetMoviesByGenre_WhenProvidedWithAnInvalidGenre_ShouldReturnHttpError()
        {
            // Act
            var invalidGenre = "xxxxxx";
            var invalidGenreException = new InvalidGenreException("Don't care");
            var httpResponseMessage = await _httpClient.GetAsync($"api/movies/genre/{invalidGenre}");

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode);
            Assert.AreEqual(invalidGenreException.Reason, httpResponseMessage.ReasonPhrase);

            var exceptionTypeHeaderValue = httpResponseMessage.Headers.GetValues("Exception-Type").First();
            Assert.IsNotNull(exceptionTypeHeaderValue, "We were expecting an HTTP Header called Exception-Type, but this header was not found");
            Assert.AreEqual(invalidGenreException.GetType().Name, exceptionTypeHeaderValue);

            var httpContent = await httpResponseMessage.Content.ReadAsStringAsync();
            AssertEx.EnsureStringContains(httpContent, invalidGenre, "not a valid Genre");
        }

        [TestMethod]
        [TestCategory("EndToEndIntegration Test")]
        public async Task CreateMovie_WhenProvidedWithAValidMovieResource_ShouldCreateMovie()
        {
            // Arrange
            var movie = RandomMovieGenerator.GenerateRandomMovies(1).Single();

            var movieResourceJson = $"{{\"title\": \"{movie.Title}\", \"ImageUrl\": \"{movie.ImageUrl}\", \"Genre\": \"{movie.Genre}\", \"Year\": {movie.Year} }}";
            var movieHttpContent = new StringContent(movieResourceJson, Encoding.UTF8, "application/json");

            // Act
            var httpResponseMessage = await _httpClient.PostAsync("api/movies", movieHttpContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode,
                "The Response content is: " + await httpResponseMessage.Content.ReadAsStringAsync() +
                $"The Movie that was used is: Title: {movie.Title}, ImageUrl: {movie.ImageUrl}, Genre: {movie.Genre}, Year: {movie.Year}");
        }

        [TestMethod]
        [TestCategory("EndToEndIntegration Test")]
        public async Task CreateMovie_WhenProvidedWithADuplicateMovieResource_ShouldThrow()
        {
            // Arrange
            var movie = RandomMovieGenerator.GenerateRandomMovies(1).Single();

            var movieResourceJson = $"{{\"title\": \"{movie.Title}\", \"ImageUrl\": \"{movie.ImageUrl}\", \"Genre\": \"{movie.Genre}\", \"Year\": {movie.Year} }}";
            var movieHttpContent = new StringContent(movieResourceJson, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("api/movies", movieHttpContent);

            // Act
            var httpResponseMessage = await _httpClient.PostAsync("api/movies", movieHttpContent);

            // Assert
            var contentString = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponseMessage.StatusCode, "The Response content is: " + contentString);
            var duplicateMovieException = new DuplicateMovieException();
            Assert.AreEqual(duplicateMovieException.Reason, httpResponseMessage.ReasonPhrase, "The Response content is: " + contentString);
            StringAssert.Contains(contentString, movie.Title);
        }
    }
}
