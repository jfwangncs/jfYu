using jfYu.Core.jfYuRequest.Enum;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
#if NETCORE
    public class JfYuHttpClient(IHttpClientFactory factory, CookieContainer cookieContainer, LogFilter logFilter, ILogger<JfYuHttpClient>? logger = null) : JfYuBaseRequest
    {
        private HttpClient? _request;
        private readonly ILogger<JfYuHttpClient>? _logger = logger;
        private readonly LogFilter _logFilter = logFilter;
        private readonly CookieContainer _cookieContainer = cookieContainer;

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
                RequestCookies.GetCookies(new Uri(Url)).ToList().ForEach(x => _cookieContainer.SetCookies(new Uri(Url), x.Name + "=" + x.Value));
                foreach (var item in RequestCustomHeaders)
                {
                    _request.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }

                CustomInitFunc?.Invoke(_request);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while initialize.");
                throw;
            }
        }

        public override async Task<string> SendAsync()
        {
            Initialize();
            try
            {
                string html = string.Empty;
                _logger?.LogInformation("Request Url:{url},Paras:{para},Method:{method}", Url, _logFilter.RequestFunc.Invoke(RequestData), Method);
                HttpResponseMessage? response = null;
                if (Method.Equals(HttpMethod.Post))
                {
                    if (ContentType == RequestContentType.FormData)
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
                _logger?.LogInformation("Response Url:{url},Result:{Result}", Url, _logFilter.ResponseFunc.Invoke(html));
                return html;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "An error occurred while sending request.");
                throw;
            }
        }

        public override async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            Initialize();

            try
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                using var response = await _request!.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead);
                StatusCode = response.StatusCode;
                if (!response.IsSuccessStatusCode)
                    return false;
                var filesize = (decimal?)response.Content.Headers.ContentLength ?? 0M;
                using var responseStream = await response.Content.ReadAsStreamAsync();
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

        public override async Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null)
        {
            Initialize();
            try
            {
                using var response = await _request!.GetAsync(Url, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                    return default;
                var filesize = (decimal?)response.Content.Headers.ContentLength ?? 0M;
                using var responseStream = await response.Content.ReadAsStreamAsync();
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
#endif
}
