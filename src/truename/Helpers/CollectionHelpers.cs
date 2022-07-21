public static class CollectionHelpers
{
    public static T[] Of<T>(this int num, Func<T> creator)
    {
        var list = new List<T>();
        for (int i = 0; i < num; i++)
            list.Add(creator());

        return list.ToArray();
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T item in source)
            action(item);
    }

    public static bool None<T>(this ICollection<T> src) =>
      !src.Any();

    public static bool None<T>(this IEnumerable<T> src, Func<T, bool> predicate) =>
      !src.Any(predicate);

    public static bool Lacks<T>(this ICollection<T> src, T val) =>
      !src.Contains(val);

    public static T After<T>(this T[] src, T val) =>
      src[Array.IndexOf(src, val) % src.Length];

    public static T After<T>(this List<T> src, T val)
    {
        var index = src.IndexOf(val);
        var position = index + 1;
        return src.NextInLoop(position);
    }

    public static T NextInLoop<T>(this ICollection<T> src, int position) =>
      src.ElementAt(position % src.Count);

    // Fisher-Yates Shuffle
    // https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    public static T[] Shuffle<T>(this T[] array)
    {
        int n = array.Length;
        T[] result = (T[])array.Clone();
        Random rng = Random.Shared;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = result[n];
            result[n] = result[k];
            result[k] = temp;
        }

        return result;
    }
}