namespace GameOfTournaments.Shared
{
    using System.Collections.Generic;
    using System.Linq;

    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> enumerable) => enumerable ?? new List<T>();

        public static IEnumerable<T> RemoveNullValues<T>(this IEnumerable<T> enumerable) => enumerable?.Where(v => v != null);

        public static IEnumerable<string> RemoveEmptyStrings(this IEnumerable<string> enumerable) => enumerable?.Where(v => !string.IsNullOrWhiteSpace(v));

        public static IEnumerable<T> EnsureCollection<T>(this IEnumerable<T> enumerable) => enumerable.OrEmptyIfNull().RemoveNullValues();

        public static List<T> EnsureCollectionToList<T>(this IEnumerable<T> enumerable) => enumerable.EnsureCollection().ToList();
    }
}