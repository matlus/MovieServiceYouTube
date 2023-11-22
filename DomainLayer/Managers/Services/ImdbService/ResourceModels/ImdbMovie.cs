using System.Diagnostics.CodeAnalysis;

namespace DomainLayer;

// Visual Studio's Code Coverage does not "see" that the setters are being covered
[ExcludeFromCodeCoverage]
internal sealed record ImdbMovie(string Title, string? ImageUrl, string? Category, int Year);