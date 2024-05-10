namespace Fischless.Linq;

public static class ForEachExtension
{
    public static  IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static IList<T> ForEach<T>(this IList<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static ICollection<T> ForEach<T>(this ICollection<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static IDictionary<TKey, TValue> ForEach<TKey, TValue>(this IDictionary<TKey, TValue> self, Action<KeyValuePair<TKey, TValue>>? action = null)
    {
        foreach (var kvp in self)
        {
            action?.Invoke(kvp);
        }
        return self;
    }

    public static IReadOnlyCollection<T> ForEach<T>(this IReadOnlyCollection<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static IReadOnlyList<T> ForEach<T>(this IReadOnlyList<T> self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static T[] ForEach<T>(this T[] self, Action<T>? action = null)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }

    public static string ForEach(this string self, Action<char>? action = null)
    {
        foreach (char item in self)
        {
            action?.Invoke(item);
        }
        return self;
    }
}
