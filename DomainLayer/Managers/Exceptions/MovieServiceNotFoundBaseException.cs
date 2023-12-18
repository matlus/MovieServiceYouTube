using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public abstract class MovieServiceNotFoundBaseException : MovieServiceBusinessBaseException
{
    protected MovieServiceNotFoundBaseException()
    {
    }

    protected MovieServiceNotFoundBaseException(string message)
        : base(message)
    {
    }

    protected MovieServiceNotFoundBaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
