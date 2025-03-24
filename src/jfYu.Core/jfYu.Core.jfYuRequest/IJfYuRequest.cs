using jfYu.Core.jfYuRequest.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Authorization
        /// </summary>
        public string Authorization { get; set; }

        /// <summary>
        /// Encoding default:utf8
        /// </summary>
        public Encoding RequestEncoding { get; set; }

        /// <summary>
        /// Request cookies
        /// </summary>
        public CookieContainer RequestCookies { get; set; }

        /// <summary>
        /// Return cookies
        /// </summary>
        public CookieCollection ResponseCookies { get; set; }

        /// <summary>
        /// Proxy only for JfYuHttpRequest
        /// </summary>
        public WebProxy? Proxy { get; set; }

        /// <summary>
        /// Upload files
        /// </summary>
        public Dictionary<string, string> Files { get; set; }

        /// <summary>
        /// Header
        /// </summary>
        public RequestHeader RequestHeader { get; set; }

        /// <summary>
        /// Timeout default:5 seconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Custom headers
        /// </summary>
        public Dictionary<string, string> RequestCustomHeaders { get; set; }

        /// <summary>
        /// Request data,you can pass string(XML,Json) or object(automatic convert to json)
        /// </summary>
        public string RequestData { get; set; }

        /// <summary>
        /// SSL only for JfYuHttpRequest，
        /// </summary>
        public X509Certificate2? Certificate { get; set; }

        /// <summary>
        /// CertificateValidation default=false only for JfYuHttpRequest
        /// </summary>
        public bool CertificateValidation { get; set; }

        /// <summary>
        /// Custom init func.for jfYuHttpRequest:object as HttpWebRequest,for jfYuHttpClient:object as HttpClient
        /// </summary>
        Action<object>? CustomInitFunc { get; set; }

        /// <summary>
        /// Response Header
        /// </summary>
        public Dictionary<string, List<string>> ResponseHeader { get; }

        /// <summary>
        /// Get response
        /// </summary> 
        /// <returns></returns>
        Task<string> SendAsync();


        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="path">file save location</param>
        /// <param name="progress">Optional progress delegate function first：Percentage of the download completed.，the second： download speed (e.g., in KB/s).,third ：estimated remaining time (e.g., in seconds)</param>
        /// <returns>successful/failed</returns>
        Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        /// <summary>
        /// Download file
        /// </summary> 
        /// <param name="progress">Optional progress delegate function first：Percentage of the download completed.，the second： download speed (e.g., in KB/s),third ：estimated remaining time (e.g., in seconds)</param>
        /// <returns>file stream</returns>
        Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);

    }
}