using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class ImdbServiceNotFoundException : MovieServiceTechnicalBaseException
{
    public override string Reason => "Imdb Service Reported - Not Found";

    public ImdbServiceNotFoundException()
    {
    }

    public ImdbServiceNotFoundException(string message)
        : base(message)
    {
    }

    public ImdbServiceNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
