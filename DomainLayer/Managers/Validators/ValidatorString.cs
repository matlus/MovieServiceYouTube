using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Managers.Validators;
internal enum StringState
{
    Null,
    Empty,
    WhiteSpaces,
    Valid
}

internal static class ValidatorString
{
    public static string? Validate(string propertyName, string propertyValue)
    {
        return DetermineNullEmptyOrWhiteSpaces(propertyValue) switch
        {
            StringState.Null => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be null",
            StringState.Empty => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be Empty",
            StringState.WhiteSpaces => $"The property: \"{propertyName}\" must be a valid {propertyName} and can not be Whitespaces",
            _ => null,
        };
    }

    public static string? Validate(string propertyName, string propertyValue, int maxLength)
    {
        var validationMessage = Validate(propertyName, propertyValue);

        return validationMessage == null && propertyValue.Length <= maxLength
            ? null
            : $"The property: \"{propertyName}\" can have a maximum length of {maxLength}";
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

        foreach (var chr in data)
        {
            if (!char.IsWhiteSpace(chr))
            {
                return StringState.Valid;
            }
        }

        return StringState.WhiteSpaces;
    }
}
