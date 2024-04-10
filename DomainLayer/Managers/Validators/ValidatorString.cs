using System.Linq;

namespace DomainLayer;

internal enum StringState
{
    Null,
    Empty,
    WhiteSpaces,
    Valid
}

internal static class ValidatorString
{
    public static string? Validate(string propertyName, string propertyValue) => DetermineNullEmptyOrWhiteSpaces(propertyValue) switch
    {
        StringState.Null => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be null",
        StringState.Empty => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be Empty",
        StringState.WhiteSpaces => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be Whitespaces",
        _ => null,
    };

    public static string? Validate(string propertyName, string propertyValue, int maxLength)
    {
        var validationMessage = Validate(propertyName, propertyValue);

        return validationMessage
            ?? (propertyValue.Length > maxLength
            ? $"The property: \"{propertyName}\" can have a maximum length of {maxLength}" 
            : null);
    }

    public static StringState DetermineNullEmptyOrWhiteSpaces(string data)
    {
        if (data == null)
        {
            return StringState.Null;
        }
        else if (data.Length == 0)
        {
            return StringState.Empty;
        }

        return data.All(char.IsWhiteSpace) ? StringState.WhiteSpaces : StringState.Valid;
    }
}
