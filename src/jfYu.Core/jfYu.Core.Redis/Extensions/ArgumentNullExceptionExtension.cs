using System;
using System.Collections.Generic;

namespace jfYu.Core.Redis.Extensions
{
    /// <summary>
    /// Provides extension methods for handling argument null exceptions.
    /// </summary>
    public static class ArgumentNullExceptionExtension
    {
        /// <summary>
        /// Throws an ArgumentNullException if the list is null or contains null elements, or if the list is empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="values">The list to check.</param>
        /// <exception cref="ArgumentNullException">Thrown when the list is null, contains null elements, or is empty.</exception>

        public static void ThrowListIfNullOrEmpty<T>(this List<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            values.ForEach(value => ArgumentNullException.ThrowIfNull(value));
            if (values.Count <= 0)
                throw new ArgumentNullException(nameof(values));
        }
    }
}
