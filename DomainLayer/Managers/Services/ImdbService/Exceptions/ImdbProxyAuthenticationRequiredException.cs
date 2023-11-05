using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[Serializable]
[ExcludeFromCodeCoverage]
public sealed class ImdbProxyAuthenticationRequiredException : MovieServiceTechnicalBaseException
{
    public override string Reason => "Proxy Authentication Required";
    public ImdbProxyAuthenticationRequiredException() { }
    public ImdbProxyAuthenticationRequiredException(string message) : base(message) { }
    public ImdbProxyAuthenticationRequiredException(string message, Exception inner) : base(message, inner) { }
    private ImdbProxyAuthenticationRequiredException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
