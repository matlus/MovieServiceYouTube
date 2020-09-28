using DomainLayer.Managers.ConfigurationProviders;
using DomainLayer.Managers.SegregatedInterfaces;
using DomainLayer.Managers.Services.ImdbService;
using System.Net.Http;

namespace DomainLayer.Managers.ServiceLocators
{
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
}
