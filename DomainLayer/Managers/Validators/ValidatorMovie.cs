using System;
using System.Collections.Generic;
using System.Linq;
using DomainLayer.Managers.Validators;

namespace DomainLayer;

internal enum StringState
{
    Null,
    Empty,
    WhiteSpaces,
    Valid,
}

internal static class ValidatorMovie
{
    private static readonly int maxLengthTitle = 50;

    public static void EnsureMovieIsValid(Movie movie)
    {
        EnsureMovieIsNotNull(movie);
        var errorMessages = ValidateProperties(movie);
        EnsureNoErrors(errorMessages);
    }

    public static void EnsureMoviesAreValid(IEnumerable<Movie> movies)
    {
        EnsureMoviesAreNotNull(movies);

        var errorMessagesSb = StringBuilderCache.Acquire();
        foreach (var movie in movies)
        {
            var errors = ValidateProperties(movie);
            if (errors != null)
            {
                errorMessagesSb.Append(errors);
            }
        }

        var errorMessages = StringBuilderCache.GetStringAndRelease(errorMessagesSb);

        if (errorMessages.Length == 0)
        {
            errorMessages = null;
        }

        EnsureNoErrors(errorMessages);
    }

    private static string? ValidateProperties(Movie movie)
    {
        var genreErrorMessage = ValidateGenre(movie.Genre);
        var titleErrorMessage = ValidatorString.Validate("Movie.Title", movie.Title, maxLengthTitle);
        var imageUrlErrorMessage = ValidatorString.Validate("Movie.ImageUrl", movie.ImageUrl);
        var yearErrorMessage = ValidateYear(movie.Year);

        var errorMessages = genreErrorMessage + titleErrorMessage + imageUrlErrorMessage + yearErrorMessage;
        return errorMessages.Length == 0 ? null : errorMessages;
    }

    private static void EnsureNoErrors(string? errorMessages)
    {
        if (errorMessages != null)
        {
            throw new InvalidMovieException(errorMessages);
        }
    }

    private static void EnsureMovieIsNotNull(Movie? movie)
    {
        if (movie == null)
        {
            throw new InvalidMovieException("The movie parameter can not be null.");
        }
    }

    private static void EnsureMoviesAreNotNull(IEnumerable<Movie> movies)
    {
        if (!movies.Any())
        {
            throw new InvalidMovieException("The movies Collection Must Contain One or More Movies and Can't be Empty.");
        }
    }

    private static string? ValidateYear(int year)
    {
        const int minimumYear = 1900;

        return year >= minimumYear && year <= DateTime.Today.Year
            ? null
            : $"The Year, must be between {minimumYear} and {DateTime.Today.Year} (inclusive)";
    }

    private static string? ValidateGenre(Genre genre)
    {
        var errorMessage = GenreParser.ValidationMessage(genre);
        if (errorMessage != null)
        {
            errorMessage += "\r\n";
        }

        return errorMessage;
    }
}