using System.Collections.Generic;

namespace jfYu.Core.Data.Model.View
{
    public class PagedModel<T>
    {
        /// <summary>
        /// 分页参数
        /// </summary>
        public QueryModel Parm { get; set; } = new QueryModel();

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// 当页第一位记录数
        /// </summary>
        public int FirstDigit { get; set; } = 0;

        /// <summary>
        /// 当页最后记录数
        /// </summary>
        public int LastDigit { get; set; } = 0;

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> List { get; set; } = new List<T>();
    }

    public class PagedModel<T, P> where P : QueryModel
    {
        /// <summary>
        /// 分页参数
        /// </summary>
        public P Parm { get; set; } = default(P);

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; } = 0;

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; } = 0;

        /// <summary>
        /// 当页第一位记录数
        /// </summary>
        public int FirstDigit { get; set; } = 0;

        /// <summary>
        /// 当页最后记录数
        /// </summary>
        public int LastDigit { get; set; } = 0;

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> List { get; set; } = new List<T>();
    }
}
