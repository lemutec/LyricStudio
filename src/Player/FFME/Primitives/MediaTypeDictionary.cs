namespace FFME.Primitives;

using Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// Represents a very simple dictionary for MediaType keys.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
[Serializable]
internal sealed class MediaTypeDictionary<TValue>
    : Dictionary<MediaType, TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaTypeDictionary{TValue}"/> class.
    /// </summary>
    public MediaTypeDictionary()
        : base(Enum.GetValues(typeof(MediaType)).Length)
    {
        // placeholder
    }

    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.", DiagnosticId = "SYSLIB0051", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    private MediaTypeDictionary(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        // placeholder
    }

    /// <summary>
    /// Gets or sets the item with the specified key.
    /// return the default value of the value type when the key does not exist.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The item.</returns>
    public new TValue this[MediaType key]
    {
        get => ContainsKey(key) == false ? default : base[key];
        internal set => base[key] = value;
    }
}
