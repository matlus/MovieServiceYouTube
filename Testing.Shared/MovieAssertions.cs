using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Shared
{
    public static class MovieAssertions
    {
        public static void AssertMoviesAreEqual(IEnumerable<Movie> expectedMovies, IEnumerable<Movie> actualMovies)
        {
            var movieEqualityComparer = new MovieEqualityComparer();
            var moviesNotInActual = expectedMovies.Except(actualMovies, movieEqualityComparer);
            var moviesNotInExpected = actualMovies.Except(expectedMovies, movieEqualityComparer);

            if (!moviesNotInActual.Any() && !moviesNotInExpected.Any())
            {
                return;
            }

            var errorMessages = new StringBuilder();
            if (moviesNotInActual.Any())
            {
                errorMessages.AppendLine("The Following movies are in Expected Movies but Not in Actual Movies");
                foreach (var movie in moviesNotInActual)
                {
                    errorMessages.AppendLine($"Title: {movie.Title}, Genre: {GenreParser.ToString(movie.Genre)}, Year: {movie.Year}");
                }
            }

            if (moviesNotInExpected.Any())
            {
                errorMessages.AppendLine("The Following movies are in Actual Movies but Not in Expected Movies");
                foreach (var movie in moviesNotInExpected)
                {
                    errorMessages.AppendLine($"Title: {movie.Title}, Genre: {GenreParser.ToString(movie.Genre)}, Year: {movie.Year}");
                }
            }

            throw new AssertFailedException(errorMessages.ToString());
        }
    }
}
