using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public abstract class MovieServiceBaseException : Exception
{
    public abstract string Reason { get; }

    protected MovieServiceBaseException()
    {
    }

    protected MovieServiceBaseException(string message)
        : base(message)
    {
    }

    protected MovieServiceBaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
