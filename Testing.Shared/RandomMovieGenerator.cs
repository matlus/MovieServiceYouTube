using System.Collections.Generic;
using DomainLayer;
using Testing.Shared.TestingHelpers;

namespace Testing.Shared;

public static class RandomMovieGenerator
{
    public static IEnumerable<Movie> GenerateRandomMovies(int count)
    {
        var randomMovies = new List<Movie>();
        for (var i = 0; i < count; i++)
        {
            randomMovies.Add(new Movie(
                    Title: RandomStringGenerator.GetRandomAciiString(50),
                    ImageUrl: RandomStringGenerator.GetRandomAciiString(200),
                    Genre: (Genre)RandomStringGenerator.GetRandomInt(0, 5),
                    Year: RandomStringGenerator.GetRandomInt(1980, 2020)));
        }

        return randomMovies;
    }
}
