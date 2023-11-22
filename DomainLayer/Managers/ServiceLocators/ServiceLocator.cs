using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

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
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };
    }

    protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
    {
        return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
    }
}