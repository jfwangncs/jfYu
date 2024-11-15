using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Data.Extension
{

    /// <summary>
    /// Pagination
    /// </summary>
    public static class PaginationExtensions
    {
        /// <summary>
        /// Pagination
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">source</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>data</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<PagedData<T>> ToPagedAsync<T>(this IQueryable<T> source, int pageIndex = 1, int pageSize = 20)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            if (pageSize <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            return new PagedData<T>() { TotalPages = totalPages, Data = list, TotalCount = totalCount };
        }

        /// <summary>
        /// Pagination
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="source">source</param>
        /// <param name="func">function</param>
        /// <param name="pageIndex">page index</param>
        /// <param name="pageSize">page size</param>
        /// <returns>data</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>       
        public static async Task<PagedData<Q>> ToPagedAsync<T, Q>(this IQueryable<T> source, Func<IEnumerable<T>, IEnumerable<Q>> func, int pageIndex = 1, int pageSize = 20)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(func);
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            if (pageSize <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            return new PagedData<Q>() { TotalPages = totalPages, Data = func(list).ToList(), TotalCount = totalCount };
        }


    }
}
