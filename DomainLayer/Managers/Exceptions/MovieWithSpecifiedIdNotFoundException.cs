﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace DomainLayer;

[Serializable]
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

    private MovieWithSpecifiedIdNotFoundException(
      SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
