// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Minor Code Smell",
    "S3963:\"static\" fields should be initialized inline",
    Justification = "The code in the static constructor needs to have run prior to this initialization",
    Scope = "member",
    Target = "~M:DomainLayer.Managers.Parsers.GenreParser.#cctor")]