using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
#if NETSTANDARD21||NET6_0||NET7_0||NET8_0
using System.Net.Http;
using System.Text;
using System.Threading;
#endif
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
#if NETSTANDARD21||NET6_0||NET7_0||NET8_0
    public class JfYuHttpClient : JfYuBaseRequest
    {


        private HttpClient? _request;

        private void Init()
        {
            var handler = new HttpClientHandler() { CookieContainer = RequestCookies, AutomaticDecompression = DecompressionMethods.GZip };
            _request = new HttpClient(handler);
            try
            {
                _request.Timeout = new TimeSpan(0, 0, Timeout);
                if (!string.IsNullOrEmpty(RequestHeader.UserAgent))
                    _request.DefaultRequestHeaders.UserAgent.ParseAdd(RequestHeader.UserAgent);
                if (!string.IsNullOrEmpty(RequestHeader.AcceptEncoding))
                    _request.DefaultRequestHeaders.AcceptEncoding.ParseAdd(RequestHeader.AcceptEncoding);
                if (!string.IsNullOrEmpty(RequestHeader.AcceptLanguage))
                    _request.DefaultRequestHeaders.AcceptLanguage.ParseAdd(RequestHeader.AcceptLanguage);
                if (!string.IsNullOrEmpty(RequestHeader.CacheControl))
                    _request.DefaultRequestHeaders.Add("CacheControl", RequestHeader.CacheControl);
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
                    _request.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Authorization.Replace("Bearer ", ""));

                if (Proxy != null)
                {
                    handler.Proxy = Proxy;
                    handler.UseProxy = true;
                }

                if (Cert != null)
                    handler.ClientCertificates.Add(Cert);

                if (!CertificateValidation)
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                foreach (var item in RequestCustomHeaders)
                {
                    _request.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                }

                CustomInitFunc?.Invoke(_request);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override async Task<string> SendAsync()
        {
            string html = "";
            Init();
            if (_request == null)
                return html;
            var paramString = GetParamString();
            try
            {
                if (Method.Equals(RequestMethod.Get))
                {
                    using var response = await _request.GetAsync($"{Url}", HttpCompletionOption.ResponseHeadersRead);
                    StatusCode = response.StatusCode;
                    foreach (var header in response.Headers)
                        ResponseHeader.Add(header.Key, header.Value.ToList());
                    string content = await response.Content.ReadAsStringAsync();
                    html = RequestEncoding.GetString(RequestEncoding.GetBytes(content));
                }
                else if (Method.Equals(RequestMethod.Post))
                {
                    using var response = await _request.PostAsync(Url, new StringContent(paramString, RequestEncoding, ContentType));
                    StatusCode = response.StatusCode;
                    foreach (var header in response.Headers)
                        ResponseHeader.Add(header.Key, header.Value.ToList());
                    string content = await response.Content.ReadAsStringAsync();
                    html = RequestEncoding.GetString(RequestEncoding.GetBytes(content));
                }
                else if (Method.Equals(RequestMethod.Put))
                {
                    using var response = await _request.PutAsync(Url, new StringContent(paramString, RequestEncoding, ContentType));
                    StatusCode = response.StatusCode;
                    foreach (var header in response.Headers)
                        ResponseHeader.Add(header.Key, header.Value.ToList());
                    string content = await response.Content.ReadAsStringAsync();
                    html = RequestEncoding.GetString(RequestEncoding.GetBytes(content));
                }
                else if (Method.Equals(RequestMethod.Delete))
                {
                    using var response = await _request.DeleteAsync($"{Url}?{paramString}");
                    StatusCode = response.StatusCode;
                    foreach (var header in response.Headers)
                        ResponseHeader.Add(header.Key, header.Value.ToList());
                    string content = await response.Content.ReadAsStringAsync();
                    html = RequestEncoding.GetString(RequestEncoding.GetBytes(content));
                }
            }
            catch (Exception)
            {
                throw;
            }

            return html;
        }



        public override async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null)
        {
            Init();
            if (_request == null)
                return false;
            var paramString = GetParamString();
            using var response = await _request.GetAsync($"{Url}?{paramString}", HttpCompletionOption.ResponseHeadersRead);
            StatusCode = response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                var filesize = decimal.Parse(response.Content.Headers.ContentLength.ToString() ?? "0");
                decimal percentage = 0M;
                decimal speed = 0M;
                decimal remain = 0;
                byte[] buffer = new byte[4096];
                try
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.CreateDirectory(dir);
                    using FileStream fs = File.Create(path);
                    long LastSaveSize = 0;
                    var t = new System.Timers.Timer(1000)
                    {
                        AutoReset = true,
                        Enabled = true
                    };
                    t.Elapsed += (s, e) =>
                    {
                        LastSaveSize = fs.Length;
                        progress?.Invoke(percentage, speed, remain);
                    };
                    t.Start();
                    int bytesRead;
                    do
                    {
                        bytesRead = await responseStream.ReadAsync(buffer);
                        await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
                        if (progress != null)
                        {
                            percentage = fs.Length / filesize * 100;
                            speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                            remain = (filesize - fs.Length) / ((speed == 0 ? 1 : speed) * 1024);
                        }
                    }
                    while (bytesRead > 0);
                    fs.Flush();
                    t.Stop();
                    progress?.Invoke(100M, 0M, 0M);
                }
                catch (Exception)
                {
                    throw;
                }

                if (File.Exists(path) && (new FileInfo(path).Length == filesize || filesize < 0))
                    return true;
                else
                    return false;
            }
            return false;
        }



        public override async Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null)
        {
            Init();
            if (_request == null)
                return default;
            var paramString = GetParamString();
            using var response = await _request.GetAsync($"{Url}?{paramString}", HttpCompletionOption.ResponseHeadersRead);
            if (response.IsSuccessStatusCode)
            {
                using var responseStream = await response.Content.ReadAsStreamAsync();
                var filesize = decimal.Parse(response.Content.Headers.ContentLength.ToString() ?? "0");
                decimal percentage = 0M;
                decimal speed = 0M;
                decimal remain = 0;
                byte[] buffer = new byte[4096];
                try
                {
                    var fs = new MemoryStream();
                    long LastSaveSize = 0;
                    var t = new System.Timers.Timer(1000)
                    {
                        AutoReset = true,
                        Enabled = true
                    };
                    t.Elapsed += (s, e) =>
                    {
                        LastSaveSize = fs.Length;
                        progress?.Invoke(percentage, speed, remain);
                    };
                    t.Start();
                    int bytesRead;
                    do
                    {
                        bytesRead = await responseStream.ReadAsync(buffer);
                        await fs.WriteAsync(buffer.AsMemory(0, bytesRead));
                        if (progress != null)
                        {
                            percentage = fs.Length / filesize * 100;
                            speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                            remain = (filesize - fs.Length) / ((speed == 0 ? 1 : speed) * 1024);
                        }
                    }
                    while (bytesRead > 0);
                    fs.Flush();
                    t.Stop();
                    progress?.Invoke(100M, 0M, 0M);
                    return fs;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return default;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
#endif
}
