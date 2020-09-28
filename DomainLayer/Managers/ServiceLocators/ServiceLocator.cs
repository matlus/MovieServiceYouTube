using System.Net.Http;
using DomainLayer.Managers.ConfigurationProviders;
using DomainLayer.Managers.Services.ImdbService;

namespace DomainLayer.Managers.ServiceLocators
{
    internal sealed class ServiceLocator : ServiceLocatorBase
    {
        protected override ConfigurationProviderBase CreateConfigurationProviderCore()
        {
            return new ConfigurationProvider();
        }

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpClientHandler();
        }

        protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
        {
            return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
        }
    }
}
