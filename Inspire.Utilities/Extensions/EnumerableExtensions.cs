namespace Inspire.Utilities.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmptyOrWhiteSpace<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any() || enumerable.All(item => item.ToString().Equals(" "));
        }

        public static bool IsNotNullOrEmptyOrWhiteSpace<T>(this IEnumerable<T> enumerable)
        {
            return !IsNullOrEmptyOrWhiteSpace(enumerable);
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !IsNullOrEmpty(enumerable);
        }

        public static IEnumerable<T> Default<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? new List<T>();
        }

        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null)
            {
                return;
            }

            var cached = items;

            foreach (var item in cached)
            {
                action(item);
            }
        }

        public static void AddSorted<T>(this List<T> @this, T item)
            where T : IComparable<T>
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return;
            }

            if (@this[@this.Count - 1].CompareTo(item) <= 0)
            {
                @this.Add(item);
                return;
            }

            if (@this[0].CompareTo(item) >= 0)
            {
                @this.Insert(0, item);
                return;
            }

            var index = @this.BinarySearch(item);
            if (index < 0)
            {
                index = ~index;
            }

            @this.Insert(index, item);
        }
    }
}