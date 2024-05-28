using jfYu.Core.jfYuRequest.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
    public interface IJfYuRequest
    {
        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// get/post default:get
        /// </summary>
        public RequestMethod Method { get; set; }

        /// <summary>
        ///  params
        /// </summary>
        public Dictionary<string, string> Params { get; set; }

        /// <summary>
        ///  raw params
        /// </summary>
        public string RawParams { get; set; }

        /// <summary>
        /// Authorization
        /// </summary>
        public string Authorization { get; set; }


        /// <summary>
        /// encoding default:utf8
        /// </summary>
        public Encoding RequestEncoding { get; set; }

        /// <summary>
        /// request cookies
        /// </summary>
        public CookieContainer RequestCookies { get; set; }

        /// <summary>
        /// return cookies
        /// </summary>
        public CookieCollection ResponseCookies { get; set; }

        /// <summary>
        /// proxy
        /// </summary>
        public WebProxy? Proxy { get; set; }

        /// <summary>
        /// upload files
        /// </summary>
        public Dictionary<string, string> Files { get; set; }

        /// <summary>
        /// header
        /// </summary>
        public RequestHeader RequestHeader { get; set; }

        /// <summary>
        /// timeout default:5 seconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// use payload default:false
        /// </summary>
        public bool UsePayload { get; set; }

        /// <summary>
        /// custom headers
        /// </summary>
        public Dictionary<string, string> RequestCustomHeaders { get; set; }

        /// <summary>
        /// ssl
        /// </summary>
        public X509Certificate2? Cert { get; set; }

        /// <summary>
        /// CertificateValidation default=false
        /// </summary>
        public bool CertificateValidation { get; set; }

        /// <summary>
        /// custom init func.for jfYuHttpRequest:object as HttpWebRequest,for jfYuHttpClient:object as HttpClient
        /// </summary>
        Action<object>? CustomInitFunc { get; set; }

        /// <summary>
        /// Response Header
        /// </summary>
        public Dictionary<string, List<string>?> ResponseHeader { get; }

        /// <summary>
        /// get html
        /// </summary>
        /// <returns>result</returns>
        Task<string> SendAsync();


        /// <summary>
        /// download file
        /// </summary>
        /// <param name="path">file save location</param>
        /// <param name="progress">progress delegate function first：download progress，the second：download speed KB/s,third ：remaining time required in seconds</param>
        /// <returns>successful/failed</returns>
        Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        /// <summary>
        /// download file
        /// </summary> 
        /// <param name="progress">progress delegate function first：download progress，the second：download speed KB/s,third ：remaining time required in seconds</param>
        /// <returns>file stream</returns>
        Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);

    }
}