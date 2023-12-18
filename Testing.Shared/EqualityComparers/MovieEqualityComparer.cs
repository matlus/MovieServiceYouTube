using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DomainLayer;

namespace Testing.Shared;

public sealed class MovieEqualityComparer : IEqualityComparer<Movie>
{
    public bool Equals([NotNullWhen(true)] Movie? x, [NotNullWhen(true)] Movie? y) =>
        x != null && y != null
        && x.Title == y.Title &&
        x.ImageUrl == y.ImageUrl &&
        x.Genre == y.Genre &&
        x.Year == y.Year;

    public int GetHashCode(Movie obj)
    {
        return HashCode.Combine(obj.Title, obj.ImageUrl, obj.Genre, obj.Year);
    }
}
