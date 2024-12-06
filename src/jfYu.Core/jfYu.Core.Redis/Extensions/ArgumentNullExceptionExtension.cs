using System;
using System.Collections.Generic;

namespace jfYu.Core.Redis.Extensions
{
    public static class ArgumentNullExceptionExtension
    {
        public static void ThrowListIfNullOrEmpty<T>(this List<T> values)
        {
            ArgumentNullException.ThrowIfNull(values);
            values.ForEach(value => ArgumentNullException.ThrowIfNull(value));
            if (values.Count <= 0)
                throw new ArgumentNullException(nameof(values));
        }
    }
}
