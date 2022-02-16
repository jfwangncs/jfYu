using System.Collections.Generic;
using System.Net;
using System.Text;

namespace jfYu.Core.jfYuRequest
{
    public abstract class jfYuBaseRequest
    {
        public jfYuBaseRequest(string url)
        {
            this.Url = url;
        }

        #region 属性
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; } = "";

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; } = "";

        /// <summary>
        /// get或者post请求，默认为get
        /// </summary>
        public jfYuRequestMethod Method { get; set; } = jfYuRequestMethod.Get;
        /// <summary>
        /// key=>value参数
        /// </summary>
        public Dictionary<string, string> Para { get; set; } = new Dictionary<string, string>();
        /// <summary>
        ///  raw参数
        /// </summary>
        public string RawPara { get; set; } = "";
        /// <summary>
        /// 字符集
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 请求cookie
        /// </summary>
        public CookieContainer Cookies { get; set; } = new CookieContainer();
        /// <summary>
        /// 请求返回的cookie
        /// </summary>
        public CookieCollection SetCookies { get; set; } = new CookieCollection();
        /// <summary>
        /// 代理
        /// </summary>
        public WebProxy Proxy { get; set; } = null;
        /// <summary>
        /// 需要上传的文件
        /// </summary>
        public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 头部请求参数
        /// </summary>
        public RequestHeader RequestHeader { get; set; } = new RequestHeader();
        /// <summary>
        /// 超时设置（秒） 默认5秒
        /// </summary>
        public int Timeout { get; set; } = 5;
        /// <summary>
        /// 失败重复次数 默认1次补重复
        /// </summary>
        public int Repetitions { get; set; } = 1;
        /// <summary>
        /// post参数是否使用Playload模式，默认不
        /// </summary>
        public bool UsePayload { get; set; } = false;
        /// <summary>
        /// 自定义header头
        /// </summary>
        public Dictionary<string, string> CustomHeader { get; set; } = new Dictionary<string, string>();
        #endregion

        #region 方法     
        /// <summary>
        /// 拼接参数
        /// </summary>  
        protected string GetParaStr()
        {
            string p = "";
            foreach (var item in Para)
                p += $"{item.Key}={item.Value}&";
            p += RawPara;
            return p;
        }
        #endregion
    }
}
