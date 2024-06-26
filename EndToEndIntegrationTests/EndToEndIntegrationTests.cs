using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieServiceCore3;
using MovieServiceCore3.ResourceModels;
using Testing.Shared;

namespace EndToEndIntegrationTests;

public sealed class MyWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override IHostBuilder? CreateHostBuilder()
    {
        return base.CreateHostBuilder();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        return base.CreateHost(builder);
    }
}

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging((context, loggingBuilder) =>
        {
            loggingBuilder.ClearProviders();
            ////loggingBuilder.AddProvider<TestContextLoggerProvider>();
        });

        builder.ConfigureServices(services =>
        {
            ////services.AddScoped<TestContextProvider>();
            ////services.AddSingleton<ILoggingSink, TestContextLoggingSink>();
            services.AddHttpContextAccessor();
        });
    }

    ////public IServiceScope CreateScope(TestContext testContext)
    ////{
    ////    var scope = this.Services.CreateScope();
    ////    var contextProvider = scope.ServiceProvider.GetRequiredService<TestContextProvider>();
    ////    contextProvider.Register(testContext);
    ////    return scope;
    ////}
}

[TestClass]
public class EndToEndIntegrationTests
{
    private static HttpClient _httpClient = default!;

    public EndToEndIntegrationTests()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        var webApplicationFactory = new CustomWebApplicationFactory();
        _httpClient = webApplicationFactory.CreateClient();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _httpClient.Dispose();
    }

    private static async Task EnsureSuccess(HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            return;
        }

        var httpContent = await httpResponseMessage.Content.ReadAsStringAsync();
        throw new AssertFailedException($"Reason Phrase: {httpResponseMessage.ReasonPhrase} with Content: {httpContent}");
    }

    [TestMethod]
    [TestCategory("EndToEndIntegration Test")]
    public async Task GetMovies_WhenOperatingNormally_ShouldSucceed()
    {
        // Act
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        var httpResponseMessage = await _httpClient.GetAsync("api/movies/");

        // Assert
        await EnsureSuccess(httpResponseMessage);
        var actualMovieResource = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<MovieResource>>();
        Assert.IsTrue(actualMovieResource!.Any());
    }

    [TestMethod]
    [TestCategory("EndToEndIntegration Test")]
    public async Task GetMoviesByGenre_WhenProvidedWithAValidAndExistingGenre_ShouldReturnMoviesForGenre()
    {
        // Act
        var validGenre = GenreParser.ToString(Genre.SciFi);
        var httpResponseMessage = await _httpClient.GetAsync($"api/movies/genre/{validGenre}");

        // Assert
        await EnsureSuccess(httpResponseMessage);
        var actualMovieResource = await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<MovieResource>>();
        Assert.IsTrue(actualMovieResource!.All(m => m.Genre == validGenre));
    }

    [TestMethod]
    [TestCategory("EndToEndIntegration Test")]
    public async Task GetMoviesByGenre_WhenProvidedWithAnInvalidGenre_ShouldReturnHttpError()
    {
        // Arrange
        var invalidGenre = "xxxxxx";
        var invalidGenreException = new InvalidGenreException($"The string: {invalidGenre} is not a valid Genre. Valid values are:");

        // Act
        var httpResponseMessage = await _httpClient.GetAsync($"api/movies/genre/{invalidGenre}");

        // Assert
        await EnsureErrorResponseIsCorrect(httpResponseMessage, HttpStatusCode.BadRequest, invalidGenreException);
    }

    [TestMethod]
    [TestCategory("EndToEndIntegration Test")]
    public async Task CreateMovie_WhenProvidedWithADuplicateMovieResource_ShouldReturnHttpError()
    {
        // Arrange
        var movie = RandomMovieGenerator.GenerateRandomMovies(1).Single();
        var duplicateMovieException = new DuplicateMovieException($"A Movie with the Title: {movie.Title} already exists.");

        var movieResource = new MovieResource(movie.Title, movie.ImageUrl, GenreParser.ToString(movie.Genre), movie.Year);
        await _httpClient.PostAsJsonAsync("api/movies", movieResource);

        // Act
        var httpResponseMessage = await _httpClient.PostAsJsonAsync("api/movies", movieResource);

        // Assert
        await EnsureErrorResponseIsCorrect(httpResponseMessage, HttpStatusCode.BadRequest, duplicateMovieException);
    }

    private static async Task EnsureErrorResponseIsCorrect(HttpResponseMessage httpResponseMessage, HttpStatusCode expectedStatusCode, MovieServiceBaseException expectedException)
    {
        var errorMessages = new StringBuilder();

        if (expectedStatusCode != httpResponseMessage.StatusCode)
        {
            errorMessages.AppendLine(CultureInfo.InvariantCulture, $"The Expected HttpStatusCode was: {HttpStatusCode.BadRequest}, but the Actual HttpStatusCode is: {httpResponseMessage.StatusCode}");
        }

        if (expectedException.Reason != httpResponseMessage.ReasonPhrase)
        {
            errorMessages.AppendLine(CultureInfo.InvariantCulture, $"The Expected Reason Phrase was: {expectedException.Reason}, but the Actual Reason Phrase is: {httpResponseMessage.ReasonPhrase}");
        }

        var expectedExceptionTypeHeaderValue = expectedException.GetType().Name;
        var actualExceptionTypeHeaderValue = httpResponseMessage.Headers.GetValues("Exception-Type").First();

        if (actualExceptionTypeHeaderValue == null)
        {
            errorMessages.AppendLine("We were expecting an HTTP Header called Exception-Type, but this header was not found");
        }
        else if (expectedExceptionTypeHeaderValue != actualExceptionTypeHeaderValue)
        {
            errorMessages.AppendLine(CultureInfo.InvariantCulture, $"The Expected \"Exception-Type\" Header Value: {expectedExceptionTypeHeaderValue}, but the Actual  \"Exception-Type\" Header Value is: {actualExceptionTypeHeaderValue}");
        }

        var actualHttpContentString = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!actualHttpContentString.Contains(expectedException.Message))
        {
            errorMessages.AppendLine(CultureInfo.InvariantCulture, $"The Expected HttpContent: {expectedException.Message}, but the Actual HttpContent is: {actualHttpContentString}");
        }

        if (errorMessages.Length > 0)
        {
            throw new AssertFailedException(errorMessages.ToString());
        }
    }

    [TestMethod]
    [TestCategory("EndToEndIntegration Test")]
    public async Task CreateMovie_WhenProvidedWithAValidMovieResource_ShouldCreateMovie()
    {
        // Arrange
        var movie = RandomMovieGenerator.GenerateRandomMovies(1).Single();

        var movieResource = new MovieResource(movie.Title, movie.ImageUrl, GenreParser.ToString(movie.Genre), movie.Year);

        // Act
        var httpResponseMessage = await _httpClient.PostAsJsonAsync("api/movies", movieResource);

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, httpResponseMessage.StatusCode, $$"""The Response content is: {{await httpResponseMessage.Content.ReadAsStringAsync()}} $"The Movie that was used is: Title: {{movie.Title}}, ImageUrl: {{movie.ImageUrl}}, Genre: {{movie.Genre}}, Year: {{movie.Year}}""");
    }
}
