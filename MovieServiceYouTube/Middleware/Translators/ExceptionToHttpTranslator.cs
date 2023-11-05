using System;
using System.Threading.Tasks;
using DomainLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace MovieServiceCore3.Middleware.Translators
{
    public static class ExceptionToHttpTranslator
    {
        public static async Task Translate(HttpContext httpContext, Exception exception)
        {
            var httpResponse = httpContext.Response;
            httpResponse.Headers.Add("Exception-Type", exception.GetType().Name);

            if (exception is MovieServiceBaseException movieServiceBaseException)
            {
                httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = movieServiceBaseException.Reason;
            }

            httpResponse.StatusCode = MapExceptionToStatusCode(exception);
            await httpResponse.WriteAsync(exception.Message);
            await httpResponse.Body.FlushAsync();
        }

        private static int MapExceptionToStatusCode(Exception exception)
        {
            if (exception is MovieServiceNotFoundBaseException)
            {
                return 404;
            }
            else if (exception is MovieServiceBusinessBaseException)
            {
                return 400;
            }

            return 500;
        }
    }
}
