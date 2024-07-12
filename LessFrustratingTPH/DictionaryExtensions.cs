using System.Collections.Generic;

namespace LessFrustratingTPH
{
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns default(T) if key not found.
        /// </summary>
        public static T TryGetValueOrDefault<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key)
        {
            T result;
            return dictionary.TryGetValue(key, out result) ? result : default(T);
        }

        /// <summary>
        /// Returns new T() if key not found.
        /// </summary>
        public static T TryGetValue<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key) where T : new()
        {
            T result;
            return dictionary.TryGetValue(key, out result) ? result : new T();
        }

        /// <summary>
        /// Throws exception if key not found, saying _which_ key was not found.
        /// </summary>
        public static T TryGetValueThrowException<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key)
        {
            T result;
            if (dictionary.TryGetValue(key, out result))
                return result;

            throw new KeyNotFoundException($"Dictionary does not contain key: {key}. Possible keys: {string.Join(",", dictionary.Keys)}");
        }


        /// <summary>
        /// Throws exception if key not found, saying _which_ key was not found.
        /// </summary>
        public static bool TryGetValueLogNotFound<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key, out T result)
        {
            if (dictionary.TryGetValue(key, out result))
                return true;

            //Main.Logger.Log($"Dictionary does not contain key: '{key}'. Possible keys: <{string.Join(",", dictionary.Keys)}>.");
            return false;
            //throw new KeyNotFoundException($"Dictionary does not contain key: {key}. Possible keys: {string.Join(",", dictionary.Keys)}");
        }

        /// <summary>
        /// Throws exception if key not found, saying _which_ key was not found.
        /// </summary>
        public static bool TryGetValueCatchException<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key, out T result)
        {
            try
            {
                if (dictionary.TryGetValue(key, out result))
                    return true;
            }
            catch (System.Exception ex)
            {
                Main.Logger.Log($"[DictExtension] Dictionary does not contain key: '{key}'. Possible keys: <{string.Join(",", dictionary.Keys)}>.");
                throw;
            }

            return false;
            //throw new KeyNotFoundException($"Dictionary does not contain key: {key}. Possible keys: {string.Join(",", dictionary.Keys)}");
        }

        // Environment fails to find the method above, hence this.
        /// <summary>
        /// Safe version of TryGetValue (for objects) which may return null without throwing an exception.
        /// </summary>
        public static T GetDictValue<T, U>(this IReadOnlyDictionary<U, T> dictionary, U key) where T : new() => TryGetValue(dictionary, key); //GetDictValue, TryGetDictValue, TryGetValueByKey
    }
}