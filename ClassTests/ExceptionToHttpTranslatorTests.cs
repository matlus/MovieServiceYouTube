using System.IO;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieServiceCore3.Middleware.Translators;

namespace ClassTests;

[TestClass]
public class ExceptionToHttpTranslatorTests
{
    [TestMethod]
    [TestCategory("Class Test")]
    public async Task Translate_WhenRequestedMovieIsNotFoundAndExceptionIsThrown_HttpStatusIs404AndHeadersAndBodyIndicateExceptionCorrectly()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        using var bodyMemoryStream = new MemoryStream();
        httpContext.Response.Body = bodyMemoryStream;

        var expectedBodyContent = "The Movie with the Specified Id was not found";
        var movieWithSpecifiedIdNotFoundException = new MovieWithSpecifiedIdNotFoundException(expectedBodyContent);

        // Act
        await ExceptionToHttpTranslator.Translate(httpContext, movieWithSpecifiedIdNotFoundException);

        // Assert
        AssertExceptionHandlingHttpResponse(httpContext, movieWithSpecifiedIdNotFoundException, 404, expectedBodyContent);
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public async Task Translate_WhenABusinessExceptionIsThrown_HttpStatusIs400AndHeadersAndBodyIndicateExceptionCorrectly()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        using var bodyMemoryStream = new MemoryStream();
        httpContext.Response.Body = bodyMemoryStream;

        var expectedBodyContent = "The movie parameter can not be null.";
        var invalidMovieException = new InvalidMovieException(expectedBodyContent);

        // Act
        await ExceptionToHttpTranslator.Translate(httpContext, invalidMovieException);

        // Assert
        AssertExceptionHandlingHttpResponse(httpContext, invalidMovieException, 400, expectedBodyContent);
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public async Task Translate_WhenATechnicalExceptionIsThrown_HttpStatusIs500AndHeadersAndBodyIndicateExceptionCorrectly()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        using var bodyMemoryStream = new MemoryStream();
        httpContext.Response.Body = bodyMemoryStream;

        var expectedBodyContent = "The Configuration Setting with Key: BaseUrl, is Missing from the Configuration file";
        var configurationSettingMissingException = new ConfigurationSettingMissingException(expectedBodyContent);

        // Act
        await ExceptionToHttpTranslator.Translate(httpContext, configurationSettingMissingException);

        // Assert
        AssertExceptionHandlingHttpResponse(httpContext, configurationSettingMissingException, 500, expectedBodyContent);
    }

    private static void AssertExceptionHandlingHttpResponse(HttpContext httpContext, MovieServiceBaseException movieServiceBaseException, int expectedStatusCode, string expectedBodyContent)
    {
        httpContext.Response.Body.Position = 0;
        using var streamReader = new StreamReader(httpContext.Response.Body);
        var actualBodyContent = streamReader.ReadToEnd();

        Assert.AreEqual(expectedBodyContent, actualBodyContent);
        Assert.AreEqual(expectedStatusCode, httpContext.Response.StatusCode);
        Assert.AreEqual(movieServiceBaseException.GetType().Name, httpContext.Response.Headers["Exception-Type"].ToString());
        Assert.AreEqual(movieServiceBaseException.Reason, httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase);
    }
}

