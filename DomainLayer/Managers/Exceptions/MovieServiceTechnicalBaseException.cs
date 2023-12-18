using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public abstract class MovieServiceTechnicalBaseException : MovieServiceBaseException
{
    protected MovieServiceTechnicalBaseException()
    {
    }

    protected MovieServiceTechnicalBaseException(string message)
        : base(message)
    {
    }

    protected MovieServiceTechnicalBaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
