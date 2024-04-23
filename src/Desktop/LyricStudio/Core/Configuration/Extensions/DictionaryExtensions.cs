using System.Collections.Generic;

namespace LyricStudio.Core.Configuration;

/// <summary>
/// Extensions for <see cref="IDictionary{TKey, TValue}"/>.
/// </summary>
internal static class DictionaryExtensions
{
    /// <summary>
    /// Add all <see cref="KeyValuePair{TKey, TValue}"/>s to dictionary.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="keyValues"><see cref="KeyValuePair{TKey, TValue}"/>s to add.</param>
    public static void AddAll<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<KeyValuePair<TKey, TValue>> keyValues) where TKey : notnull
    {
        foreach (var pair in keyValues)
            dictionary.Add(pair.Key, pair.Value);
    }
}
