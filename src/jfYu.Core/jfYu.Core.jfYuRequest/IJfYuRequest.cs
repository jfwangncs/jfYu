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
    /// <summary>
    /// Interface for JfYuRequest
    /// </summary>
    public interface IJfYuRequest
    {
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ContentType,default:Josn
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Http Method <see cref="HttpMethod"/> class.
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
        /// Request cookies <see cref="CookieContainer"/> class.
        /// </summary>
        public CookieContainer RequestCookies { get; set; }

        /// <summary>
        /// Return cookies <see cref="CookieCollection"/> class.
        /// </summary>
        public CookieCollection ResponseCookies { get; set; }

        /// <summary>
        /// Proxy only for JfYuHttpRequest <see cref="WebProxy"/> class.
        /// </summary>
        public WebProxy? Proxy { get; set; }

        /// <summary>
        /// Upload files
        /// </summary>
        public Dictionary<string, string> Files { get; set; }

        /// <summary>
        /// Header <see cref="RequestHeader"/> class.
        /// </summary>
        public RequestHeader RequestHeader { get; set; }

        /// <summary>
        /// Timeout default:30 seconds
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Custom headers
        /// </summary>
        public Dictionary<string, string> RequestCustomHeaders { get; set; }

        /// <summary>
        /// Request data,
        /// Json:"{\"username\":\"testUser\"}"
        /// FormData/FormUrlEncoded:"username=testUser"
        /// Xml:"<user><username>testUser</username></user>"
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
        /// Custom init.for jfYuHttpRequest:object as HttpWebRequest,for jfYuHttpClient:object as HttpClient
        /// </summary>
        Action<object>? CustomInit { get; set; }

        /// <summary>
        /// Response Header
        /// </summary>
        public Dictionary<string, List<string>> ResponseHeader { get; }

        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Sends the HTTP request asynchronously.
        /// </summary>
        /// <returns>The response content.</returns>
        Task<string> SendAsync();

        /// <summary>
        /// Downloads a file asynchronously.
        /// </summary>
        /// <param name="path">The file save location.</param>
        /// <param name="progress">The progress delegate function.</param>
        /// <returns>True if the download is successful, otherwise false.</returns>
        Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        /// <summary>
        /// Downloads a file to a memory stream asynchronously.
        /// </summary>
        /// <param name="progress">The progress delegate function.</param>
        /// <returns>The memory stream containing the downloaded file.</returns>
        Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);
    }
}