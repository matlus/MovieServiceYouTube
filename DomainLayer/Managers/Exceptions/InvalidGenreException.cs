using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class InvalidGenreException : MovieServiceBusinessBaseException
{
    public override string Reason => "Invalid Genre";

    public InvalidGenreException()
    {
    }

    public InvalidGenreException(string message)
        : base(message)
    {
    }

    public InvalidGenreException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
