using jfYu.Core.jfYuRequest.Enum;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable SYSLIB0014
namespace jfYu.Core.jfYuRequest
{

    /// <summary>
    /// Represents an HTTP request with logging and additional features.(Using HttpWebRequest)
    /// </summary>
    public class JfYuHttpRequest : JfYuBaseRequest
    {
        /// <summary>
        /// The HTTP web request.
        /// </summary>
        private HttpWebRequest? _request;
 
        private readonly ILogger<JfYuHttpRequest>? _logger;

        private readonly LogFilter _logFilter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JfYuHttpRequest"/> class.
        /// </summary>
        /// <param name="logFilter">The log filter.</param>
        /// <param name="logger">The logger.</param>
        public JfYuHttpRequest(LogFilter logFilter, ILogger<JfYuHttpRequest>? logger = null)
        {
            _logFilter = logFilter;
            _logger = logger;
        }

        /// <inheritdoc/>
        private void Initialize()
        {
            _request = (HttpWebRequest)WebRequest.Create(Url);
            _request.Method = Method.ToString().ToUpper();

            if (!Method.Equals(HttpMethod.Get))
            {
                if (ContentType == RequestContentType.FormData)
                {
                    var memStream = new MemoryStream();
                    string boundary = "--" + DateTime.Now.Ticks.ToString("x");
                    if (!string.IsNullOrEmpty(RequestData))
                    {
                        var paras = RequestData;
                        string[] parameters = paras.Split('&');
                        foreach (string param in parameters)
                        {
                            string[] keyValue = param.Split('=');
                            if (keyValue.Length == 2)
                            {
                                string key = keyValue[0];
                                string value = keyValue[1];
                                var paramByte = RequestEncoding.GetBytes($"\r\n--{boundary}\r\nContent-Disposition: form-data; name=\"{key}\"\r\n\r\n{value}");
                                memStream.Write(paramByte, 0, paramByte.Length);
                            }
                        }
                    }
                    foreach (var item in Files)
                    {
                        string filePath = item.Value;
                        byte[] fileData = File.ReadAllBytes(filePath);

                        var fileContentStrByte = Encoding.UTF8.GetBytes($"\r\n--{boundary}\r\n" +
                            $"Content-Disposition: form-data; name=\"{item.Key}\"; filename=\"{Path.GetFileName(item.Value)}\"\r\n" +
                            $"Content-Type:application/octet-stream\r\n\r\n");
                        memStream.Write(fileContentStrByte, 0, fileContentStrByte.Length);
                        memStream.Write(fileData, 0, fileData.Length);

                    }
                    var endBoundary = Encoding.UTF8.GetBytes($"\r\n--{boundary}--\r\n");
                    memStream.Write(endBoundary, 0, endBoundary.Length);
                    byte[] tempBuffer = memStream.ToArray();
                    _request.ContentLength = tempBuffer.Length;
                    _request.ContentType = "multipart/form-data;boundary=" + boundary;
                    using var reqStream = _request.GetRequestStream();
                    reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                }
                else
                {
                    _request.ContentType = ContentType;
                    var data = RequestEncoding.GetBytes(RequestData);
                    _request.ContentLength = data.Length;
                    using var requestStream = _request.GetRequestStream();
                    requestStream.Write(data, 0, data.Length);
                }
            }

            try
            {
                _request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                _request.CookieContainer = RequestCookies;
                _request.Timeout = Timeout * 1000;
                _request.UserAgent = RequestHeader.UserAgent;
                _request.Headers.Add(HttpRequestHeader.AcceptEncoding, RequestHeader.AcceptEncoding);// Define gzip compression support
                _request.Headers.Add(HttpRequestHeader.AcceptLanguage, RequestHeader.AcceptLanguage);
                _request.Headers.Add(HttpRequestHeader.CacheControl, RequestHeader.CacheControl);
                _request.Headers.Add(HttpRequestHeader.Pragma, RequestHeader.Pragma);
                if ("keep-alive".Equals(RequestHeader.Connection))
                    _request.KeepAlive = true;
                if (RequestHeader.Host != "")
                    _request.Host = RequestHeader.Host.Replace("https://", "").Replace("http://", "");
                _request.Referer = RequestHeader.Referer;
                _request.Accept = RequestHeader.Accept;

                if (Proxy != null)
                    _request.Proxy = Proxy;

                if (!string.IsNullOrEmpty(Authorization))
                    _request.Headers.Add("Authorization", $"Bearer {Authorization.Replace("Bearer ", "")}");

                if (Certificate != null)
                {
                    CertificateValidation = true;
                    _request.ClientCertificates.Add(Certificate);
                }

                if (!CertificateValidation)
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                foreach (var item in RequestCustomHeaders)
                {
                    _request.Headers.Add(item.Key, item.Value);
                }

                CustomInit?.Invoke(_request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while initialize.");
                throw;
            }
        }

        /// <summary>
        /// Gets the response content as a string.
        /// </summary>
        /// <param name="response">The HTTP web response.</param>
        /// <returns>The response content.</returns>
        private string GetResponse(HttpWebResponse response)
        {
            if (response.ContentEncoding.ToLower().Contains("br") || response.ContentEncoding.ToLower().Contains("brotli"))
            {
                using var brotliStream = new Brotli.BrotliStream(response.GetResponseStream(), CompressionMode.Decompress);
                using var brotliReader = new StreamReader(brotliStream, RequestEncoding);
                return brotliReader.ReadToEnd();
            }
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream, RequestEncoding);
            return reader.ReadToEnd();
        }


        /// <inheritdoc/>
        public override async Task<string> SendAsync()
        {
            var html = string.Empty;
            var requestId = Guid.NewGuid().ToString();
            try
            {
                Initialize();
                _logger?.LogInformation("{Message}",LogRequest(_logFilter.LoggingFields, requestId, Url, Method.ToString(), JsonConvert.SerializeObject(_request!.Headers.AllKeys.ToDictionary(header => header, header => _request.Headers.GetValues(header)!.ToList())), _logFilter.RequestFilter.Invoke(RequestData)));
                HttpWebResponse response = (HttpWebResponse)await _request!.GetResponseAsync();
                StatusCode = response.StatusCode;
                html = GetResponse(response);
                ResponseCookies = response.Cookies;
                ResponseHeader = response.Headers.AllKeys.ToDictionary(header => header, header => response.Headers.GetValues(header)!.ToList());
                response.Close();
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var response = (HttpWebResponse)e.Response;
                    html = GetResponse(response);
                    StatusCode = response.StatusCode;
                }
                else
                {
                    _logger?.LogError(e, "An error occurred while sending request.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while sending request.");
                throw;
            }

            _logger?.LogInformation("{Message}", LogResponse(_logFilter.LoggingFields, requestId, StatusCode.ToString(), _logFilter.ResponseFilter.Invoke(html)));
            return html;
        }


        /// <inheritdoc/>
        public override async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            Initialize();
            try
            {
                HttpWebResponse response = (HttpWebResponse)await _request!.GetResponseAsync();
                StatusCode = response.StatusCode;
                using Stream responseStream = response.GetResponseStream();
                ResponseCookies = response.Cookies;
                var filesize = (decimal)response.ContentLength;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, DefaultBufferSize, FileOptions.Asynchronous);
                await DownloadFileInternalAsync(fileStream, responseStream, filesize, progress);
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while downloading the file.");
                throw;
            }
        }


        /// <inheritdoc/>
        public override async Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null)
        {
            Initialize();
            try
            {
                HttpWebResponse response = (HttpWebResponse)await _request!.GetResponseAsync();
                StatusCode = response.StatusCode;
                using Stream responseStream = response.GetResponseStream();
                ResponseCookies = response.Cookies;
                var filesize = decimal.Parse(response.ContentLength.ToString());
                var memoryStream = new MemoryStream();
                await DownloadFileInternalAsync(memoryStream, responseStream, filesize, progress);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while downloading the file.");
                throw;
            }
        }
    }
}
