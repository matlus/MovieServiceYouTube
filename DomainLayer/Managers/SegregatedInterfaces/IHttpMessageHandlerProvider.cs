using System.Net.Http;

namespace DomainLayer.Managers.SegregatedInterfaces
{
    internal interface IHttpMessageHandlerProvider
    {
        HttpMessageHandler CreateHttpMessageHandler();
    }
}
