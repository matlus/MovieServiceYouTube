using System.Net.Http;

namespace DomainLayer;

internal interface IHttpMessageHandlerProvider
{
    HttpMessageHandler CreateHttpMessageHandler();
}
