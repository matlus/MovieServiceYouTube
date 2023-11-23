using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DomainLayer;

[Serializable]
[ExcludeFromCodeCoverage]
public sealed class ImdbProxyAuthenticationRequiredException : MovieServiceTechnicalBaseException
{
    public override string Reason => "Proxy Authentication Required";

    public ImdbProxyAuthenticationRequiredException()
    {
    }

    public ImdbProxyAuthenticationRequiredException(string message)
        : base(message)
    {
    }

    public ImdbProxyAuthenticationRequiredException(string message, Exception inner)
        : base(message, inner)
    {
    }

    private ImdbProxyAuthenticationRequiredException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
