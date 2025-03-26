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
    public abstract class JfYuBaseRequest : IJfYuRequest
    {

        public string Url { get; set; } = "";
        public string ContentType { get; set; } = RequestContentType.Json;
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public string Authorization { get; set; } = "";
        public Encoding RequestEncoding { get; set; } = Encoding.UTF8;
        public CookieContainer RequestCookies { get; set; } = new();
        public CookieCollection ResponseCookies { get; set; } = [];
        public WebProxy? Proxy { get; set; }
        public Dictionary<string, string> Files { get; set; } = [];
        public RequestHeader RequestHeader { get; set; } = new();
        public int Timeout { get; set; } = 30;
        public Dictionary<string, string> RequestCustomHeaders { get; set; } = [];
        public X509Certificate2? Certificate { get; set; }
        public bool CertificateValidation { get; set; } = false;
        public HttpStatusCode StatusCode { get; protected set; }
        public Action<object>? CustomInitFunc { get; set; }
        public Dictionary<string, List<string>> ResponseHeader { get; protected set; } = [];
        public string RequestData { get; set; } = "";
        protected const int DefaultBufferSize = 8192;
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

        protected static string LogResponse(JfYuLoggingFields LoggingFields, string requestId, string status, string result)
        {

            var logMessage = new StringBuilder($"Response [RequestId:{requestId}]");

            if ((LoggingFields & JfYuLoggingFields.ResponseStatus) == JfYuLoggingFields.ResponseStatus)
                logMessage.Append($" Status={status}");

            if ((LoggingFields & JfYuLoggingFields.Response) == JfYuLoggingFields.Response)
                logMessage.Append($" Result={result}");

            return logMessage.ToString();
        }
        public abstract Task<string> SendAsync();

        public abstract Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        public abstract Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);

    }
}
