using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class DuplicateMovieException : MovieServiceBusinessBaseException
{
    public override string Reason => "Duplicate Movie";

    public DuplicateMovieException()
    {
    }

    public DuplicateMovieException(string message)
        : base(message)
    {
    }

    public DuplicateMovieException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
