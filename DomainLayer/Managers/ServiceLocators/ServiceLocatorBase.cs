using System.Net.Http;

namespace DomainLayer;

internal abstract class ServiceLocatorBase : IHttpMessageHandlerProvider
{
    public ImdbServiceGateway CreateImdbServiceGateway()
    {
        return CreateImdbServiceGatewayCore();
    }

    public ConfigurationProviderBase CreateConfigurationProvider()
    {
        return CreateConfigurationProviderCore();
    }

    public HttpMessageHandler CreateHttpMessageHandler()
    {
        return CreateHttpMessageHandlerCore();
    }

    protected abstract HttpMessageHandler CreateHttpMessageHandlerCore();

    protected abstract ConfigurationProviderBase CreateConfigurationProviderCore();

    protected abstract ImdbServiceGateway CreateImdbServiceGatewayCore();
}
