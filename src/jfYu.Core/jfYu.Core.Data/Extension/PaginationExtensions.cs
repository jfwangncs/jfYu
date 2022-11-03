using jfYu.Core.Data.Model.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Data.Extension
{

    public static class PaginationExtensions
    {
        public static PagedModel<T> ToPaged<T>(this IQueryable<T> source, int pageIndex = 1, int pageSize = 20)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = source.Count();
            var list = source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<T>() { TotalPages = totalPages, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static async Task<PagedModel<T>> ToPagedAsync<T>(this IQueryable<T> source, int pageIndex = 1, int pageSize = 20)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<T>() { TotalPages = totalPages, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static PagedModel<Q> ToPaged<T, Q>(this IQueryable<T> source, Func<IEnumerable<T>, IEnumerable<Q>> func, int pageIndex = 1, int pageSize = 20)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = source.Count();
            var list = source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<Q>() { TotalPages = totalPages, List = func(list).ToList(), FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static async Task<PagedModel<Q>> ToPagedAsync<T, Q>(this IQueryable<T> source, Func<IEnumerable<T>, IEnumerable<Q>> func, int pageIndex = 1, int pageSize = 20)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<Q>() { TotalPages = totalPages, List = func(list).ToList(), FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }


        public static PagedModel<T, P> ToPaged<T, P>(this IQueryable<T> source, int pageIndex = 1, int pageSize = 20) where P : QueryModel
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = source.Count();
            var list = source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<T, P>() { TotalPages = totalPages, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static async Task<PagedModel<T,P>> ToPagedAsync<T,P>(this IQueryable<T> source, int pageIndex = 1, int pageSize = 20) where P : QueryModel
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<T,P>() { TotalPages = totalPages, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static PagedModel<Q,P> ToPaged<T, Q,P>(this IQueryable<T> source, Func<IEnumerable<T>, IEnumerable<Q>> func, int pageIndex = 1, int pageSize = 20) where P : QueryModel
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = source.Count();
            var list = source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToList();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<Q,P>() { TotalPages = totalPages, List = func(list).ToList(), FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
        public static async Task<PagedModel<Q,P>> ToPagedAsync<T, Q,P>(this IQueryable<T> source, Func<IEnumerable<T>, IEnumerable<Q>> func, int pageIndex = 1, int pageSize = 20) where P : QueryModel
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            if (pageIndex <= 0)
                throw new InvalidOperationException($"{nameof(pageIndex)} must be a positive integer greater than 0.");

            int totalCount = await source.CountAsync();
            var list = await source.Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            int startNum = pageSize * (pageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<Q,P>() { TotalPages = totalPages, List = func(list).ToList(), FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };
        }
    }
}
