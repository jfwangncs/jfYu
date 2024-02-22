using System.Collections.Generic;

namespace jfYu.Core.Data.Model
{
    /// <summary>
    /// Paged Data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedData<T>
    {
        /// <summary>
        /// Total Pages
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// Total Count
        /// </summary>
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// data
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();
    }
}
