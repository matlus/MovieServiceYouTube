using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public abstract class MovieServiceBusinessBaseException : MovieServiceBaseException
{
    protected MovieServiceBusinessBaseException()
    {
    }

    protected MovieServiceBusinessBaseException(string message)
        : base(message)
    {
    }

    protected MovieServiceBusinessBaseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
