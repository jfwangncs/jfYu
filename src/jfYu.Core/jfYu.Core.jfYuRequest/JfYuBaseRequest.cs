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
    ///Implement the basic properties and methods of the IJfYuRequest
    /// </summary>
    public abstract class JfYuBaseRequest : IJfYuRequest
    {
        /// <inheritdoc/>
        public string Url { get; set; } = "";

        /// <inheritdoc/>
        public string ContentType { get; set; } = RequestContentType.Json;

        /// <inheritdoc/>
        public HttpMethod Method { get; set; } = HttpMethod.Get;

        /// <inheritdoc/>
        public string Authorization { get; set; } = "";

        /// <inheritdoc/>
        public Encoding RequestEncoding { get; set; } = Encoding.UTF8;

        /// <inheritdoc/>
        public CookieContainer RequestCookies { get; set; } = new();

        /// <inheritdoc/>
        public CookieCollection ResponseCookies { get; set; } = [];

        /// <inheritdoc/>
        public WebProxy? Proxy { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, string> Files { get; set; } = [];

        /// <inheritdoc/>
        public RequestHeader RequestHeader { get; set; } = new();

        /// <inheritdoc/>
        public int Timeout { get; set; } = 30;

        /// <inheritdoc/>
        public Dictionary<string, string> RequestCustomHeaders { get; set; } = [];

        /// <inheritdoc/>
        public string RequestData { get; set; } = "";

        /// <inheritdoc/>
        public X509Certificate2? Certificate { get; set; }

        /// <inheritdoc/>
        public bool CertificateValidation { get; set; } = false;

        /// <inheritdoc/>
        public Action<object>? CustomInit { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, List<string>> ResponseHeader { get; protected set; } = [];

        /// <inheritdoc/>
        public HttpStatusCode StatusCode { get; protected set; }

        /// <inheritdoc/>
        protected const int DefaultBufferSize = 8192;

        /// <summary>
        /// Downloads a file asynchronously to a target stream.
        /// </summary>
        /// <param name="targetStream">The stream to write the downloaded file to.</param>
        /// <param name="responseStream">The stream to read the file from.</param>
        /// <param name="fileSize">The size of the file being downloaded.</param>
        /// <param name="progress">The progress delegate function.</param>
        /// <returns>The target stream containing the downloaded file.</returns>
        protected static async Task<Stream> DownloadFileInternalAsync(Stream targetStream, Stream responseStream, decimal fileSize, Action<decimal, decimal, decimal>? progress)
        {
            try
            {
                decimal lastSaveSize = 0;
                byte[] buffer = new byte[DefaultBufferSize];
                int bytesRead;
                do
                {
                    bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                    await targetStream.WriteAsync(buffer, 0, bytesRead);
                    if (progress != null && fileSize > 0)
                    {
                        decimal currentLength = targetStream.Length;
                        decimal percentage = currentLength / fileSize * 100;
                        decimal speed = (currentLength - lastSaveSize) / 1024;
                        decimal remain = speed == 0 ? 0 : (fileSize - currentLength) / (speed * 1024);
                        progress.Invoke(percentage, speed, remain);
                        lastSaveSize = currentLength;
                    }
                }
                while (bytesRead > 0);
                targetStream.Flush();
                progress?.Invoke(100M, 0M, 0M);
                return targetStream;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Logs the request details.
        /// </summary>
        /// <param name="LoggingFields">The fields to be included in the logging.</param>
        /// <param name="requestId">The unique identifier for the request.</param>
        /// <param name="url">The request URL.</param>
        /// <param name="method">The HTTP method used for the request.</param>
        /// <param name="header">The request headers.</param>
        /// <param name="requestData">The request data.</param>
        /// <returns>The formatted log message.</returns>
        protected static string LogRequest(JfYuLoggingFields LoggingFields, string requestId, string url, string method, string header, string requestData)
        {
            var logMessage = new StringBuilder($"Request [RequestId:{requestId}]");

            if ((LoggingFields & JfYuLoggingFields.RequestPath) == JfYuLoggingFields.RequestPath)
                logMessage.Append($" Path={url}");

            if ((LoggingFields & JfYuLoggingFields.RequestMethod) == JfYuLoggingFields.RequestMethod)
                logMessage.Append($" Method={method}");

            if ((LoggingFields & JfYuLoggingFields.RequestHeaders) == JfYuLoggingFields.RequestHeaders)
                logMessage.Append($" Headers={header}");

            if ((LoggingFields & JfYuLoggingFields.RequestData) == JfYuLoggingFields.RequestData)
                logMessage.Append($" Data={requestData}");

            return logMessage.ToString();
        }

        /// <summary>
        /// Logs the response details.
        /// </summary>
        /// <param name="LoggingFields">The fields to be included in the logging.</param>
        /// <param name="requestId">The unique identifier for the request.</param>
        /// <param name="status">The response status.</param>
        /// <param name="result">The response content.</param>
        /// <returns>The formatted log message.</returns>
        protected static string LogResponse(JfYuLoggingFields LoggingFields, string requestId, string status, string result)
        {
            var logMessage = new StringBuilder($"Response [RequestId:{requestId}]");

            if ((LoggingFields & JfYuLoggingFields.ResponseStatus) == JfYuLoggingFields.ResponseStatus)
                logMessage.Append($" Status={status}");

            if ((LoggingFields & JfYuLoggingFields.Response) == JfYuLoggingFields.Response)
                logMessage.Append($" Result={result}");

            return logMessage.ToString();
        }

        /// <inheritdoc/>
        public abstract Task<string> SendAsync();

        /// <inheritdoc/>
        public abstract Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        /// <inheritdoc/>

        public abstract Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);
    }
}