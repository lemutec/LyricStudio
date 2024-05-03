namespace Fischless.Linq;

public static class ForEachExtension
{
    public static void ForEach<T>(this IEnumerable<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach<T>(this IList<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach<T>(this ICollection<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> self, Action<KeyValuePair<TKey, TValue>>? action = null)
    {
        foreach (var kvp in self)
        {
            action?.Invoke(kvp);
        }
    }

    public static void ForEach<T>(this IReadOnlyCollection<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach<T>(this IReadOnlyList<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach<T>(this T[] self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static void ForEach(this string self, Action<char>? action = null)
    {
        foreach (char item in self)
        {
            action?.Invoke(item);
        }
    }
}
