using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class ConfigurationSettingMissingException : MovieServiceTechnicalBaseException
{
    public override string Reason => "Configuration Setting Missing";

    public ConfigurationSettingMissingException()
    {
    }

    public ConfigurationSettingMissingException(string message)
        : base(message)
    {
    }

    public ConfigurationSettingMissingException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
