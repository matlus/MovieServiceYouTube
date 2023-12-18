using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class InvalidMovieException : MovieServiceBusinessBaseException
{
    public override string Reason => "Invalid Movie";

    public InvalidMovieException()
    {
    }

    public InvalidMovieException(string message)
        : base(message)
    {
    }

    public InvalidMovieException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
