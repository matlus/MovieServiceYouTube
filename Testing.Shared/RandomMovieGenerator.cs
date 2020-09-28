using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using System.Collections.Generic;
using Testing.Shared.TestingHelpers;

namespace Testing.Shared
{
    public static class RandomMovieGenerator
    {
        public static IEnumerable<Movie> GenerateRandomMovies(int count)
        {
            var randomMovies = new List<Movie>();
            for (int i = 0; i < count; i++)
            {
                randomMovies.Add(new Movie
                    (
                        title: RandomStringGenerator.GetRandomAciiString(50),
                        imageUrl: RandomStringGenerator.GetRandomAciiString(200),
                        genre: (Genre)RandomStringGenerator.GetRandomInt(0, 5),
                        year: RandomStringGenerator.GetRandomInt(1980, 2020)
                    ));
            }

            return randomMovies;
        }
    }
}
