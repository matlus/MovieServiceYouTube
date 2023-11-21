using System;
using System.Net.Http;
using AcceptanceTests.TestDoubles.Spies;
using AcceptanceTests.TestMediators;
using DomainLayer;

namespace AcceptanceTests.DomainLayer.Managers.ServiceLocators;

internal sealed class ServiceLocatorForAcceptanceTesting : ServiceLocatorBase
{
    private readonly TestMediator _testMediator;
    public ServiceLocatorForAcceptanceTesting(TestMediator testMediator) => _testMediator = testMediator;

    protected override ConfigurationProviderBase CreateConfigurationProviderCore() => new ConfigurationProvider();

    protected override HttpMessageHandler CreateHttpMessageHandlerCore()
    {
        // For Testing Purposes, you need only return an instance of the HttpMessageHandlerSpy (with the test mediator constructor parameter
        // If during testing there are situations where certain test scenarios require you to "intercept" using the spy
        // while others require you to "forward" the call onto the actual HttpMessageHandler
        // then the delegating handler with the InnerHandler as the actual (SocketsHttpHandler) might be needed
        var socketsHttpHandler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        var myDelegatingHander = new HttpMessageHandlerSpy(_testMediator);
        myDelegatingHander.InnerHandler = socketsHttpHandler;
        return myDelegatingHander;
    }

    protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
    {
        return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
    }
}
