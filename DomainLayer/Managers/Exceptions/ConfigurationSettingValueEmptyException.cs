using System;
using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

[ExcludeFromCodeCoverage]
public sealed class ConfigurationSettingValueEmptyException : MovieServiceTechnicalBaseException
{
    public override string Reason => "Configuration Setting Value Empty";

    public ConfigurationSettingValueEmptyException()
    {
    }

    public ConfigurationSettingValueEmptyException(string message)
        : base(message)
    {
    }

    public ConfigurationSettingValueEmptyException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
