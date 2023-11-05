using System.Collections.Generic;
using System.Reflection;

namespace DomainLayer;

public static class GenreParser
{
    private static readonly Dictionary<string, Genre> StringToGenreMappings = new();
    private static readonly Dictionary<Genre, string> GenreToStringMappings = new();
    private static readonly string JoinedGenres;

    public static IEnumerable<string> GenreValues
    {
        get
        {
            foreach (KeyValuePair<Genre, string> kvp in GenreToStringMappings)
            {
                yield return kvp.Value;
            }
        }
    }

    static GenreParser()
    {
        FieldInfo[] fieldInfos = typeof(Genre).GetFields(BindingFlags.Public | BindingFlags.Static);

        foreach (FieldInfo fieldInfo in fieldInfos)
        {
            var genre = (Genre)fieldInfo.GetValue(null)!;

            var enumDescriptionAttributes = (EnumDescriptionAttribute[])fieldInfo
                .GetCustomAttributes(typeof(EnumDescriptionAttribute), false);

            if (enumDescriptionAttributes.Length == 0)
            {
                StringToGenreMappings.Add(fieldInfo.Name.ToLower(System.Threading.Thread.CurrentThread.CurrentCulture), genre);
                GenreToStringMappings.Add(genre, fieldInfo.Name);
            }
            else
            {
                GenreToStringMappings.Add(genre, enumDescriptionAttributes[0].Description);

                foreach (EnumDescriptionAttribute enumDescriptionAttribute in enumDescriptionAttributes)
                {
                    StringToGenreMappings.Add(enumDescriptionAttribute.Description.ToLower(System.Threading.Thread.CurrentThread.CurrentCulture), genre);
                }
            }
        }

        JoinedGenres = string.Join(',', GenreValues);
    }

    public static Genre Parse(string genreAsString)
    {
        if (string.IsNullOrEmpty(genreAsString))
        {
            throw new InvalidGenreException($"The string can not be null or empty. Valid values are: {JoinedGenres}");
        }

        var genreAsStringLowered = genreAsString.ToLower(System.Threading.Thread.CurrentThread.CurrentCulture);

        return StringToGenreMappings.TryGetValue(genreAsStringLowered, out Genre genre)
            ? genre
            : throw new InvalidGenreException($"The string: {genreAsString} is not a valid Genre. Valid values are: {JoinedGenres}");
    }

    public static string ToString(Genre genre)
    {
        Validate(genre);
        return GenreToStringMappings[genre];
    }

    public static void Validate(Genre genre)
    {
        var validationMessage = ValidationMessage(genre);

        if (validationMessage != null)
        {
            throw new InvalidGenreException(validationMessage);
        }
    }

    public static string? ValidationMessage(Genre genre)
    {
        return !GenreToStringMappings.ContainsKey(genre) ? $"The Genre: {genre} is not a valid Genre. Valid values are: {JoinedGenres}" : null;
    }
}
