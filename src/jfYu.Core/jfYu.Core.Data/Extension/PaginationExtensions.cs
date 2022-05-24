using System;
using System.Linq;
using System.Threading.Tasks;
using jfYu.Core.Data.Model.View;
using Microsoft.EntityFrameworkCore;

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
    }
}
