using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using AcceptanceTests.TestMediators;
using DomainLayer;

namespace AcceptanceTests.TestDoubles.Spies;

internal sealed class HttpMessageHandlerSpy(TestMediator testMeditor) : DelegatingHandler
{
    private readonly TestMediator _testMediator = testMeditor;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_testMediator.ExceptionInformation == null)
        {
            var movies = _testMediator.MoviesForGetAllMovies!;
            return WriteMoviesToResponse(request, movies);
        }
        else
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                StatusCode = (HttpStatusCode)_testMediator.ExceptionInformation.ExceptionReason,
                Content = new StringContent("Exception Occured")
            };

            return Task.FromResult(httpResponseMessage);
        }
    }

    private static Task<HttpResponseMessage> WriteMoviesToResponse(HttpRequestMessage request, IEnumerable<Movie> movies)
    {
        IEnumerable<ImdbMovie> imdbMovies = default!;

        var absolutePath = request.RequestUri!.AbsolutePath;

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

        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(imdbMovies)
        };

        return Task.FromResult(httpResponseMessage);
    }
}
