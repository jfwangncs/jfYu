﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using jfYu.Core.jfYuRequest.Enum;
using System.Threading;

namespace jfYu.Core.jfYuRequest
{
#if NETCORE

    /// <summary>
    /// Represents an HTTP request with logging and additional features.(Using HttpClient)
    /// </summary>
    /// <param name="factory"> Initializes a new instance of the <see cref="IHttpClientFactory"/> class.</param>
    /// <param name="cookieContainer">The cookie container</param>
    /// <param name="logFilter">The log filter.</param>
    /// <param name="logger">The logger.</param>
    public class JfYuHttpClient(IHttpClientFactory factory, CookieContainer cookieContainer, LogFilter logFilter, ILogger<JfYuHttpClient>? logger = null) : JfYuBaseRequest
    {
        /// <summary>
        /// The HTTP web request.
        /// </summary>
        private HttpClient? _request;

        private readonly ILogger<JfYuHttpClient>? _logger = logger;

        private readonly LogFilter _logFilter = logFilter;

        /// <summary>
        /// The cookie container
        /// </summary>

        private readonly CookieContainer _cookieContainer = cookieContainer;

        /// <inheritdoc/>
        private void Initialize()
        {
            try
            {
                _request = factory.CreateClient("httpclient");
                _request.Timeout = TimeSpan.FromSeconds(Timeout);
                if (!string.IsNullOrEmpty(RequestHeader.UserAgent))
                    _request.DefaultRequestHeaders.UserAgent.ParseAdd(RequestHeader.UserAgent);
                if (!string.IsNullOrEmpty(RequestHeader.AcceptEncoding))
                    _request.DefaultRequestHeaders.AcceptEncoding.ParseAdd(RequestHeader.AcceptEncoding);
                if (!string.IsNullOrEmpty(RequestHeader.AcceptLanguage))
                    _request.DefaultRequestHeaders.AcceptLanguage.ParseAdd(RequestHeader.AcceptLanguage);
                if (!string.IsNullOrEmpty(RequestHeader.CacheControl))
                    _request.DefaultRequestHeaders.Add("Cache-Control", RequestHeader.CacheControl);
                if (!string.IsNullOrEmpty(RequestHeader.Pragma))
                    _request.DefaultRequestHeaders.Pragma.ParseAdd(RequestHeader.Pragma);
                if ("keep-alive".Equals(RequestHeader.Connection))
                    _request.DefaultRequestHeaders.ConnectionClose = false;
                if (!string.IsNullOrEmpty(RequestHeader.Host))
                    _request.DefaultRequestHeaders.Host = RequestHeader.Host.Replace("https://", "").Replace("http://", "");
                if (!string.IsNullOrEmpty(RequestHeader.Referer))
                    _request.DefaultRequestHeaders.Referrer = new Uri(RequestHeader.Referer);
                if (!string.IsNullOrEmpty(RequestHeader.Accept))
                    _request.DefaultRequestHeaders.Accept.ParseAdd(RequestHeader.Accept);

                if (!string.IsNullOrEmpty(Authorization))
                    _request.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Authorization.Replace("Bearer ", ""));
                ;
                RequestCookies.GetCookies(new Uri(Url)).ToList().ForEach(x => _cookieContainer.SetCookies(new Uri(Url), $"{x.Name}={x.Value}"));
                foreach (var item in RequestCustomHeaders)
                {
                    _request.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }

                CustomInit?.Invoke(_request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while initialize.");
                throw;
            }
        }

        /// <inheritdoc/>
        public override async Task<string> SendAsync()
        {
            Initialize();
            var requestId = Guid.NewGuid().ToString();
            try
            {
                string html = string.Empty;
                if (_logFilter.LoggingFields != JfYuLoggingFields.None)
                    _logger?.LogInformation("{Message}", LogRequest(_logFilter.LoggingFields, requestId, Url, Method.ToString(), JsonConvert.SerializeObject(_request!.DefaultRequestHeaders.ToDictionary(header => header.Key, header => header.Value.ToList())), _logFilter.RequestFilter.Invoke(RequestData)));

                HttpResponseMessage? response = null;
                if (Method.Equals(HttpMethod.Post))
                {
                    if (string.Compare(ContentType, RequestContentType.FormData, StringComparison.Ordinal) == 0)
                    {
                        using var formData = new MultipartFormDataContent();
                        if (!string.IsNullOrEmpty(RequestData))
                        {
                            var paras = RequestData;
                            string[] parameters = paras.Split('&');
                            foreach (string param in parameters)
                            {
                                string[] keyValue = param.Split('=');
                                if (keyValue.Length == 2)
                                    formData.Add(new StringContent(keyValue[1]), keyValue[0]);
                            }
                        }

                        foreach (var file in Files)
                        {
                            var fileStream = File.OpenRead(file.Value);
                            var fileContent = new StreamContent(fileStream);
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            formData.Add(fileContent, file.Key, Path.GetFileName(file.Value));
                        }
                        response = await _request!.PostAsync(Url, formData);
                    }
                    else
                        response = await _request!.PostAsync(Url, new StringContent(RequestData, RequestEncoding, ContentType));
                }
                else if (Method.Equals(HttpMethod.Put))
                    response = await _request!.PutAsync(Url, new StringContent(RequestData, RequestEncoding, ContentType));
                else if (Method.Equals(HttpMethod.Delete))
                    response = await _request!.DeleteAsync(Url);
                else
                    response = await _request!.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead);
                ResponseHeader = response.Headers.ToDictionary(header => header.Key, header => header.Value.ToList());
                StatusCode = response.StatusCode;
                _cookieContainer.GetCookies(new Uri(Url)).ToList().ForEach(ResponseCookies.Add);
                string content = await response.Content.ReadAsStringAsync();
                html = RequestEncoding.GetString(RequestEncoding.GetBytes(content));
                if (_logFilter.LoggingFields != JfYuLoggingFields.None)
                    _logger?.LogInformation("{Message}", LogResponse(_logFilter.LoggingFields, requestId, StatusCode.ToString(), _logFilter.ResponseFilter.Invoke(html)));
                return html;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while sending request.");
                throw;
            }
        }

        /// <inheritdoc/>
        public override async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            Initialize();

            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                using var response = await _request!.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                StatusCode = response.StatusCode;
                if (!response.IsSuccessStatusCode)
                    return false;
                var filesize = (decimal?)response.Content.Headers.ContentLength ?? 0M;
                using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, DefaultBufferSize, FileOptions.Asynchronous);
                await DownloadFileInternalAsync(fileStream, responseStream, filesize, progress, cancellationToken);
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while downloading the file.");
                throw;
            }
        }

        /// <inheritdoc/>
        public override async Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null, CancellationToken cancellationToken = default)
        {
            Initialize();
            try
            {
                using var response = await _request!.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                if (!response.IsSuccessStatusCode)
                    return default;
                var filesize = (decimal?)response.Content.Headers.ContentLength ?? 0M;
                using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var memoryStream = new MemoryStream();
                await DownloadFileInternalAsync(memoryStream, responseStream, filesize, progress, cancellationToken);
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
#endif
}