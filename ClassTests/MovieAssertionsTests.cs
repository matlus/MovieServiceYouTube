using System.Collections.Generic;
using DomainLayer.Managers.Enums;
using DomainLayer.Managers.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Shared;

namespace ClassTests
{
    [TestClass]
    public class MovieAssertionsTests
    {
        [TestMethod]
        [TestCategory("Class Test")]
        public void AssertMoviesAreEqual_WhenExpectedAndActualAreEqual_NoExceptionIsThrown()
        {
            // Arrange
            var expectedMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.Drama, 1901),
            };

            var actualMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.Drama, 1901),
            };

            // Act
            MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);

            // Assert
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public void AssertMoviesAreEqual_WhenExpectedHasMoviesNotInActual_Throws()
        {
            // Arrange
            var expectedMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.SciFi, 1901),
                new Movie("D", "ImageUrlC", Genre.Thriller, 1902),
                new Movie("E", "ImageUrlE", Genre.Comedy, 1903),
            };

            var actualMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.Drama, 1901),
            };

            // Act
            try
            {
                MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
            }
            catch (AssertFailedException e)
            {
                // Assert
                AssertEx.EnsureExceptionMessageContains(e,
                    "The Following movies are in Expected Movies but Not in Actual Movies",
                    "Title: C, Genre: Sci-Fi, Year: 1901",
                    "Title: D, Genre: Thriller, Year: 1902",
                    "Title: E, Genre: Comedy, Year: 1903",
                    "The Following movies are in Actual Movies but Not in Expected Movies",
                    "Title: C, Genre: Drama, Year: 1901");
            }
        }

        [TestMethod]
        [TestCategory("Class Test")]
        public void AssertMoviesAreEqual_WhenActualHasMoviesNotInExpected_Throws()
        {
            // Arrange
            var expectedMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.Drama, 1901),
            };

            var actualMovies = new List<Movie>
            {
                new Movie("A", "ImageUrlA", Genre.Action, 1900),
                new Movie("C", "ImageUrlB", Genre.SciFi, 1901),
                new Movie("D", "ImageUrlC", Genre.Thriller, 1902),
                new Movie("E", "ImageUrlE", Genre.Comedy, 1903),
            };

            // Act
            try
            {
                MovieAssertions.AssertMoviesAreEqual(expectedMovies, actualMovies);
            }
            catch (AssertFailedException e)
            {
                // Assert
                var expectedExceptionMessage = "The Following movies are in Expected Movies but Not in Actual Movies\r\n" +
                                               "Title: C, Genre: Drama, Year: 1901\r\n" +
                                               "The Following movies are in Actual Movies but Not in Expected Movies\r\n" +
                                               "Title: C, Genre: Sci-Fi, Year: 1901\r\n" +
                                               "Title: D, Genre: Thriller, Year: 1902\r\n" +
                                               "Title: E, Genre: Comedy, Year: 1903\r\n";
                Assert.AreEqual(expectedExceptionMessage, e.Message);
            }
        }
    }
}
