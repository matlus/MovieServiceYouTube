using System;
using System.Net.Http;

namespace DomainLayer;

internal sealed class ServiceLocator : ServiceLocatorBase
{
    protected override ConfigurationProviderBase CreateConfigurationProviderCore()
    {
        return new ConfigurationProvider();
    }

    protected override HttpMessageHandler CreateHttpMessageHandlerCore()
    {
        return new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        };
    }

    protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
    {
        return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
    }
}