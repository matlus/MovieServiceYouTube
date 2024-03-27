using System;

namespace DomainLayer;

public enum Genre
{
    Action,
    Comedy,
    Drama,
    [EnumDescription("Sci-Fi")]
    [EnumDescription("SciFi")]
    SciFi,
    Thriller,
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
internal sealed class EnumDescriptionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}
