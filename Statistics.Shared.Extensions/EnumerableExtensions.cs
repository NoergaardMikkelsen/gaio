namespace Statistics.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        return source.SelectMany(sublist => sublist).ToList();
    }
}
