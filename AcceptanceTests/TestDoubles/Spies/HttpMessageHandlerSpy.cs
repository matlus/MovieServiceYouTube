using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AcceptanceTests.TestMediators;
using DomainLayer;
using Newtonsoft.Json;

namespace AcceptanceTests.TestDoubles.Spies;

internal sealed class HttpMessageHandlerSpy : HttpMessageHandler
{
    private readonly TestMediator _testMediator;

    public HttpMessageHandlerSpy(TestMediator testMeditor) => _testMediator = testMeditor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_testMediator.ExceptionInformation == null)
        {
            var movies = _testMediator.MoviesForGetAllMovies;
            return WriteMoviesToResponse(request, movies);
        }
        else
        {
            var httpResponseMessage = request.CreateResponse();
            httpResponseMessage.StatusCode = (HttpStatusCode)_testMediator.ExceptionInformation.ExceptionReason;
            httpResponseMessage.Content = new StringContent("Exception Occured");
            return Task.FromResult(httpResponseMessage);
        }
    }

    private static Task<HttpResponseMessage> WriteMoviesToResponse(HttpRequestMessage request, IEnumerable<Movie> movies)
    {
        IEnumerable<ImdbMovie> imdbMovies = null;

        var absolutePath = request.RequestUri.AbsolutePath;

        if (absolutePath.Contains("WithCategories.json"))
        {
            imdbMovies = movies.Select(m => new ImdbMovie(m.Title, ImageUrl: null, GenreParser.ToString(m.Genre), Year: 0));
        }
        else if (absolutePath.Contains("WithImageUrls.json"))
        {
            imdbMovies = movies.Select(m => new ImdbMovie(m.Title, ImageUrl: m.ImageUrl, Category: null, Year: 0));
        }
        else if (absolutePath.Contains("WithYears.json"))
        {
            imdbMovies = movies.Select(m => new ImdbMovie(m.Title, ImageUrl: null, Category: null, Year: m.Year));
        }

        var imdbMoviesJson = JsonConvert.SerializeObject(imdbMovies);
        var httpResponseMessage = request.CreateResponse();
        httpResponseMessage.Content = new StringContent(imdbMoviesJson, Encoding.UTF8, "application/json");

        return Task.FromResult(httpResponseMessage);
    }
}
