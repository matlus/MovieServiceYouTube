using DomainLayer.Managers.Models;
using DomainLayer.Managers.Parsers;
using DomainLayer.Managers.SegregatedInterfaces;
using DomainLayer.Managers.Services.ImdbService.Exceptions;
using DomainLayer.Managers.Services.ImdbService.ResourceModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DomainLayer.Managers.Services.ImdbService
{
    internal sealed class ImdbServiceGateway : IDisposable
    {
        private bool _disposed;
        private HttpClient _httpClient;
        private readonly IHttpMessageHandlerProvider _httpMessageHandlerProvider;

        public ImdbServiceGateway(IHttpMessageHandlerProvider httpMessageHandlerProvider, string baseUrl)
        {
            _httpMessageHandlerProvider = httpMessageHandlerProvider;
            _httpClient = CreateHttpClient(baseUrl);
        }

        private HttpClient CreateHttpClient(string baseUrl)
        {
            var httpMessageHandler = MakeHttpMessageHandler();

            if (httpMessageHandler is HttpClientHandler httpClientHandler)
            {
                httpClientHandler.PreAuthenticate = true;
                httpClientHandler.DefaultProxyCredentials = CredentialCache.DefaultCredentials;
            }

            var httpClient = new HttpClient(httpMessageHandler)
            {
                BaseAddress = new Uri(baseUrl),
            };

            return httpClient;
        }

        private HttpMessageHandler MakeHttpMessageHandler()
        {
            return _httpMessageHandlerProvider.CreateHttpMessageHandler();
        }

        public async Task<IEnumerable<Movie>> GetAllMovies()
        {
            var httpResponseMessages =
                await MakeConcurrentGetAsyncCalls(_httpClient, "withcategories", "withimageurls", "withyears").ConfigureAwait(false);

            await EnsureSuccessForAllTasks(httpResponseMessages).ConfigureAwait(false);

            IEnumerable<ImdbMovie>[] arrayOfEnumerableOfImdbMovies =
                await GetMoviesFromHttpContent(httpResponseMessages[0].Content, httpResponseMessages[1].Content, httpResponseMessages[2].Content).ConfigureAwait(false);

            var moviesWithCategory = arrayOfEnumerableOfImdbMovies[0];
            var moviesWithImgUrl = arrayOfEnumerableOfImdbMovies[1];
            var moviesWithYear = arrayOfEnumerableOfImdbMovies[2];

            return JoinImdbMoviesMapToMovies(moviesWithCategory, moviesWithImgUrl, moviesWithYear);
        }

        private static IEnumerable<Movie> JoinImdbMoviesMapToMovies(IEnumerable<ImdbMovie> moviesWithCategory, IEnumerable<ImdbMovie> moviesWithImgUrl, IEnumerable<ImdbMovie> moviesWithYear)
        {
            var allMovies = new List<Movie>();

            using (var moviesWithCategoryEnumerator = moviesWithCategory.GetEnumerator())
            using (var moviesWithImgUrlEnumerator = moviesWithImgUrl.GetEnumerator())
            using (var moviesWithYearEnumerator = moviesWithYear.GetEnumerator())
            {
                while (moviesWithCategoryEnumerator.MoveNext())
                {
                    var movieWithCategory = moviesWithCategoryEnumerator.Current;

                    moviesWithImgUrlEnumerator.MoveNext();
                    var movieWithImgUrl = moviesWithImgUrlEnumerator.Current;

                    moviesWithYearEnumerator.MoveNext();
                    var movieWithYear = moviesWithYearEnumerator.Current;

                    allMovies.Add(new Movie(
                            title: movieWithCategory.Title,
                            imageUrl: movieWithImgUrl.ImageUrl,
                            genre: GenreParser.Parse(movieWithCategory.Category),
                            year: movieWithYear.Year));
                }
            }

            return allMovies;
        }

        private static Task<IEnumerable<ImdbMovie>[]> GetMoviesFromHttpContent(HttpContent content1, HttpContent content2, HttpContent content3)
        {
            var moviesWithCategoryTask = content1.ReadAsAsync<IEnumerable<ImdbMovie>>();
            var moviesWithImgUrlTask = content2.ReadAsAsync<IEnumerable<ImdbMovie>>();
            var moviesWithYearTask = content3.ReadAsAsync<IEnumerable<ImdbMovie>>();

            return Task.WhenAll(moviesWithCategoryTask, moviesWithImgUrlTask, moviesWithYearTask);
        }

        private static Task EnsureSuccessForAllTasks(HttpResponseMessage[] httpResponseMessages)
        {
            var ensureSuccessTask1 = EnsureSuccess(httpResponseMessages[0].IsSuccessStatusCode, httpResponseMessages[0].StatusCode, httpResponseMessages[0].Content);
            var ensureSuccessTask2 = EnsureSuccess(httpResponseMessages[1].IsSuccessStatusCode, httpResponseMessages[1].StatusCode, httpResponseMessages[1].Content);
            var ensureSuccessTask3 = EnsureSuccess(httpResponseMessages[2].IsSuccessStatusCode, httpResponseMessages[2].StatusCode, httpResponseMessages[2].Content);

            return Task.WhenAll(ensureSuccessTask1, ensureSuccessTask2, ensureSuccessTask3);
        }

        private static Task<HttpResponseMessage[]> MakeConcurrentGetAsyncCalls(HttpClient httpClient, string withCategoriesEndpoint, string withImageUrlsEndpoint, string withYearsEndpoint)
        {
            var httpResponseMessageTask1 = httpClient.GetAsync(withCategoriesEndpoint);
            var httpResponseMessageTask2 = httpClient.GetAsync(withImageUrlsEndpoint);
            var httpResponseMessageTask3 = httpClient.GetAsync(withYearsEndpoint);

            return Task.WhenAll(httpResponseMessageTask1, httpResponseMessageTask2, httpResponseMessageTask3);
        }

        private static async Task EnsureSuccess(bool isSuccessStatusCode, HttpStatusCode statusCode, HttpContent content)
        {
            if (isSuccessStatusCode)
            {
                return;
            }

            var httpContent = await content.ReadAsStringAsync().ConfigureAwait(false);

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new ImdbServiceNotFoundException("The Imdb Service call resulted in a Not Found Status Code");
                case HttpStatusCode.ProxyAuthenticationRequired:
                    throw new ImdbProxyAuthenticationRequiredException($"The Imdb Service call resulted in status code of: {statusCode}, with body: {httpContent}");
                default:
                    throw new ImdbServiceNotFoundException($"The Imdb Service call resulted in a status code of: {statusCode}, with body: {httpContent}");
            }
        }

        [ExcludeFromCodeCoverage]
        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed && _httpClient != null)
            {
                var localHttpClient = _httpClient;
                localHttpClient.Dispose();
                _httpClient = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
