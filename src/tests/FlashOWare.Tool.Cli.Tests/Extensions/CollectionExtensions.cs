namespace FlashOWare.Tool.Cli.Tests.Extensions;

internal static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, T[] items)
    {
        foreach (T item in items)
        {
            collection.Add(item);
        }
    }
}
