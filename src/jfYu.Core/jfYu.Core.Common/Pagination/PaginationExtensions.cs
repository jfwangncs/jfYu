using System;
using System.Linq;
using System.Threading.Tasks;

namespace jfYu.Core.Common.Pagination
{

    public static class PaginationExtensions
    {

        public static async Task<PagedModel<T>> ToPagingAsync<T, Q>(this IQueryable<T> source, Q parm) where Q : QueryModel
        {

            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (parm == null)
                throw new ArgumentNullException(nameof(parm));

            var list = source.Skip(parm.PageSize * (parm.PageIndex - 1)).Take(parm.PageSize).ToList();
            int startNum = parm.PageSize * (parm.PageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            int totalCount = source.Count();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / parm.PageSize);
            await Task.Delay(1);
            return new PagedModel<T>() { TotalPages = totalPages, Parm = parm, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };

        }
        public static PagedModel<T> ToPaging<T, Q>(this IQueryable<T> source, Q parm) where Q : QueryModel
        {
            if (source == null)
                throw new ArgumentNullException();
            int totalCount = source.Count();
            var list = source.Skip(parm.PageSize * (parm.PageIndex - 1)).Take(parm.PageSize).ToList();
            int totalPages = (int)Math.Ceiling((decimal)totalCount / parm.PageSize);
            int startNum = parm.PageSize * (parm.PageIndex - 1) + 1;
            int endNum = startNum + list.Count - 1;
            return new PagedModel<T>() { TotalPages = totalPages, Parm = parm, List = list, FirstDigit = startNum, LastDigit = endNum, TotalCount = totalCount };

        }
    }
}
