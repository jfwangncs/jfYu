using System;
using System.IO;
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
    public interface IjfYuRequest
    {
        /// <summary>
        /// 异步获取网页内容
        /// </summary>
        /// <returns>网页内容</returns>
        Task<string> SendAsync();


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存地址</param>
        /// <param name="setProgress">进度、速度、所需时间匿名回调函数，第一个参数：为下载进度，第二个参数：下载速度默认单位KB/s,第三个参数：剩余所需时间单位秒</param>
        Task<bool> DownloadFileAsync(string savePath, Action<decimal, decimal, decimal> setProgress = null);


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">文件保存地址</param>
        /// <param name="setProgress">进度、速度、所需时间匿名回调函数，第一个参数：为下载进度，第二个参数：下载速度默认单位KB/s,第三个参数：剩余所需时间单位秒</param>
        Task<MemoryStream> DownloadFileAsync(Action<decimal, decimal, decimal> setProgress = null);

    }
}