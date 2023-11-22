using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

// Visual Studio's Code Coverage does not "see" that the setters are being covered
[ExcludeFromCodeCoverage]
public sealed record Movie(string Title, string ImageUrl, Genre Genre, int Year);