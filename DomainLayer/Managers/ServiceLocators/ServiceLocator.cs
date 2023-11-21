using System;
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
        var socketsHttpHandler = new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        var myDelegatingHander = new MyDelegatingHandlerSpy();
        myDelegatingHander.InnerHandler = socketsHttpHandler;
        return myDelegatingHander;
    }

    protected override ImdbServiceGateway CreateImdbServiceGatewayCore()
    {
        return new ImdbServiceGateway(this, CreateConfigurationProvider().GetImdbServiceBaseUrl());
    }
}

internal sealed class MyDelegatingHandlerSpy : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Not calling the inner handler becasue we're intercepting the call
        ////return base.SendAsync(request, cancellationToken);
        var httpResponseMessage = new HttpResponseMessage()
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            RequestMessage = request,
            Content = new StringContent("[{\"Title\": \"Avatar\", \"Category\": \"SciFi\" }]", new MediaTypeHeaderValue("application/json")),
        };

        return Task.FromResult(httpResponseMessage);
    }
}