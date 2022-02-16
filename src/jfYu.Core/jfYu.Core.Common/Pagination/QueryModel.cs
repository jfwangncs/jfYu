using System;
using System.ComponentModel;

namespace jfYu.Core.Common.Pagination
{

    public class QueryModel
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [DisplayName("当前页码")]
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 页面尺寸
        /// </summary>
        [DisplayName("页面尺寸")]
        public int PageSize { get; set; } = 8;

        /// <summary>
        ///搜索关键字
        /// </summary>
        [DisplayName("搜索关键字")]
        public String Key { get; set; } = "";

    }
}
