using AcceptanceTests.TestDoubles.Spies;
using AcceptanceTests.TestMediators;
using DomainLayer.Managers.ConfigurationProviders;
using DomainLayer.Managers.ServiceLocators;
using DomainLayer.Managers.Services.ImdbService;
using System.Net.Http;

namespace AcceptanceTests.DomainLayer.Managers.ServiceLocators
{
    internal sealed class ServiceLocatorForAcceptanceTesting : ServiceLocatorBase
    {
        private readonly TestMediator _testMediator;
        public ServiceLocatorForAcceptanceTesting(TestMediator testMediator)
        {
            _testMediator = testMediator;
        }

        protected override ConfigurationProviderBase CreateConfigurationProviderCore() => new ConfigurationProvider();

        protected override HttpMessageHandler CreateHttpMessageHandlerCore()
        {
            return new HttpMessageHandlerSpy(_testMediator);
        }

        protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
        {
            return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
        }
    }
}
