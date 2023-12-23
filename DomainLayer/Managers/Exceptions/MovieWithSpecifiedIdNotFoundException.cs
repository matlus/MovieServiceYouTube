using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class MovieWithSpecifiedIdNotFoundException : MovieServiceNotFoundBaseException
{
    public override string Reason => "Movie with Specified Id Not Found";

    public MovieWithSpecifiedIdNotFoundException()
    {
    }

    public MovieWithSpecifiedIdNotFoundException(string message)
        : base(message)
    {
    }

    public MovieWithSpecifiedIdNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
