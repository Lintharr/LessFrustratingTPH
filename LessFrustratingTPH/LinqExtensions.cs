using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LessFrustratingTPH
{
    public static class LinqExtensions
    {
        public static bool ContainsOneOf(this string value, params string[] collection)
        {
            foreach (var item in collection)
            {
                if (value.Contains(item))
                    return true;
            }

            return false;
        }

        public static bool IsIn<T>(this T value, params T[] collection) => collection.Contains(value);

        public static bool IsIn<T>(this T value, IEnumerable<T> collection) => collection.Contains(value);

        public static bool IsNotIn<T>(this T value, params T[] collection) => value.IsIn(collection) == false;

        public static bool IsNotIn<T>(this T value, IEnumerable<T> collection) => value.IsIn(collection) == false;

        public static bool OnlyOneElementSelected(this ICollection collection) => collection != null && collection.Count == 1;

        public static string ListThis<T>(this IEnumerable<T> list, string listName, bool addCount = false, string separator = ", ", string wrapChars = "<>")
            => $"<b>{listName}{(addCount ? $" ({list.Count()})" : "")}</b>: {wrapChars[0]}{string.Join(separator, list)}{wrapChars[1]}";
    }

    public static class Enum
    {
        public static IEnumerable<TEnum> GetValues<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
                => System.Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        public static string[] GetNames<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
                => System.Enum.GetNames(typeof(TEnum));

        public static IEnumerable<string> GetKeys<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
                => System.Enum.GetNames(typeof(TEnum));

        public static TEnum Parse<TEnum>(string value)
            where TEnum : struct, IConvertible, IComparable, IFormattable
                => (TEnum)System.Enum.Parse(typeof(TEnum), value);
    }
}