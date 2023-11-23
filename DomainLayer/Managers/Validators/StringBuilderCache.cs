using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DomainLayer;

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable enable
/// <summary>Provide a cached reusable instance of stringbuilder per thread.</summary>
[ExcludeFromCodeCoverage]
internal static class StringBuilderCache
{
    // The value 360 was chosen in discussion with performance experts as a compromise between using
    // as little memory per thread as possible and still covering a large part of short-lived
    // StringBuilder creations on the startup path of VS designers.
    internal const int MaxBuilderSize = 360;
    private const int DefaultCapacity = 16; // == StringBuilder.DefaultCapacity

    [ThreadStatic]
    private static StringBuilder? _cachedInstance;

    /// <summary>
    /// Get a StringBuilder for the specified capacity.
    /// </summary>
    /// <param name="capacity">The capacity of the StringBuilder.</param>
    /// <returns>A Cached instance of a StringBuilder.</returns>
    /// <remarks>If a StringBuilder of an appropriate size is cached, it will be returned and the cache emptied.</remarks>
    public static StringBuilder Acquire(int capacity = DefaultCapacity)
    {
        if (capacity <= MaxBuilderSize)
        {
            var sb = _cachedInstance;

            // Avoid stringbuilder block fragmentation by getting a new StringBuilder
            // when the requested size is larger than the current capacity
            if (sb != null && capacity <= sb.Capacity)
            {
                _cachedInstance = null;
                sb.Clear();
                return sb;
            }
        }

        return new StringBuilder(capacity);
    }

    /// <summary>
    /// Place the specified builder in the cache if it is not too big.
    /// </summary>
    /// <param name="sb">An instance of a StringBuilder that was previously retrived using <seealso cref="Acquire"/>.</param>
    public static void Release(StringBuilder sb)
    {
        if (sb.Capacity <= MaxBuilderSize)
        {
            _cachedInstance = sb;
        }
    }

    /// <summary>
    /// ToString() the stringbuilder, Release it to the cache, and return the resulting string.
    /// </summary>
    /// <param name="sb">An instance of a StringBuilder that was previously retrived using <seealso cref="Acquire"/>.</param>
    /// <returns>Returns the string contained in the StringBuilder.</returns>
    public static string GetStringAndRelease(StringBuilder sb)
    {
        var stringContent = sb.ToString();
        Release(sb);
        return stringContent;
    }
}
