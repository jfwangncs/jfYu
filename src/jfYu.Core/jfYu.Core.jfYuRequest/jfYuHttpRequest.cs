using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using jfYu.Core.jfYuRequest.Enum;

namespace jfYu.Core.jfYuRequest
{
    public class JfYuHttpRequest : JfYuBaseRequest
    {
        private HttpWebRequest? _request;

        private void Init()
        {
            var paramString = GetParamString();

            if (Method.Equals(HttpMethod.Get))
            {
                _request = (HttpWebRequest)WebRequest.Create($"{Url}");
                _request.Method = Method.ToString().ToUpper();
            }
            else
            {
                _request = (HttpWebRequest)WebRequest.Create(Url);
                _request.Method = Method.ToString().ToUpper();
                if (UsePayload)
                {
                    var memStream = new MemoryStream();
                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    string Enter = "\r\n";
                    foreach (var item in Params)
                    {
                        string pstr = Enter + "--" + boundary + Enter
                      + "Content-Disposition: form-data; name=\"" + item.Key + "\"" + Enter + Enter
                      + item.Value;
                        var StrByte = RequestEncoding.GetBytes(pstr);
                        memStream.Write(StrByte, 0, StrByte.Length);
                    }
                    foreach (var file in Files)
                    {
                        var fs = new FileStream(file.Value, FileMode.Open, FileAccess.Read);
                        var fileContentByte = new byte[fs.Length];
                        fs.Read(fileContentByte, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        string fileContentStr = Enter + "--" + boundary + Enter
                       + $"Content-Disposition: form-data; name=\"{file.Key}\"; filename=\"{Path.GetFileName(file.Value)}\""
                       + Enter + "Content-Type:application/octet-stream" + Enter + Enter;
                        var fileContentStrByte = Encoding.UTF8.GetBytes(fileContentStr);
                        memStream.Write(fileContentStrByte, 0, fileContentStrByte.Length);
                        memStream.Write(fileContentByte, 0, fileContentByte.Length);

                    }
                    var endBoundary = Encoding.UTF8.GetBytes(Enter + "--" + boundary + "--" + Enter);
                    memStream.Write(endBoundary, 0, endBoundary.Length);
                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    _request.ContentLength = tempBuffer.Length;
                    _request.ContentType = "multipart/form-data;boundary=" + boundary;
                    using Stream reqStream = _request.GetRequestStream();
                    reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                    reqStream.Close();
                }
                else
                {
                    _request.ContentType = ContentType == "" ? RequestContentType.XWWWFormUrlEncoded : ContentType;
                    byte[] tempBuffer = RequestEncoding.GetBytes(paramString);
                    _request.ContentLength = tempBuffer.Length;
                    using Stream reqStream = _request.GetRequestStream();
                    reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                    reqStream.Close();
                }
            }

            if (_request == null)
                throw new NullReferenceException("init failed. request is null");

            _request.ContentType = ContentType;
            _request.CookieContainer = RequestCookies;
            try
            {
                _request.Timeout = Timeout * 1000;
                _request.UserAgent = RequestHeader.UserAgent;
                _request.Headers.Add(HttpRequestHeader.AcceptEncoding, RequestHeader.AcceptEncoding);//定义gzip压缩页面支持
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

                if (Cert != null)
                    _request.ClientCertificates.Add(Cert);

                if (!CertificateValidation)
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                foreach (var item in RequestCustomHeaders)
                {
                    _request.Headers.Add(item.Key, item.Value);
                }

                CustomInitFunc?.Invoke(_request);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetResponseBody(HttpWebResponse response)
        {
            string responseBody = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(response.ContentEncoding))
                {
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        using var stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using var reader = new StreamReader(stream, this.RequestEncoding);
                        responseBody = reader.ReadToEnd();
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using var stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using var reader = new StreamReader(stream, this.RequestEncoding);
                        responseBody = reader.ReadToEnd();
                    }
                    else if (response.ContentEncoding.ToLower().Contains("br") || response.ContentEncoding.ToLower().Contains("brotli"))
                    {
                        using var stream = new Brotli.BrotliStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using var reader = new StreamReader(stream, this.RequestEncoding);
                        responseBody = reader.ReadToEnd();
                    }
                }
                else
                {
                    using var stream = response.GetResponseStream();
                    using var reader = new StreamReader(stream, RequestEncoding);
                    responseBody = reader.ReadToEnd();
                }

            }
            catch (Exception)
            {
                using var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream, RequestEncoding);
                responseBody = reader.ReadToEnd();
            }
            responseBody = responseBody.Replace("\0", "");
            return responseBody;
        }

        public override async Task<string> SendAsync()
        {
            Init();
            if (_request == null)
                throw new NullReferenceException("init failed. request is null");
            try
            {
                using var response = (HttpWebResponse)await _request.GetResponseAsync();
                StatusCode = response.StatusCode;
                if (response == null)
                    throw new WebException("response null");

                var html = GetResponseBody(response);
                ResponseCookies = response.Cookies;
                foreach (var header in response.Headers.AllKeys)
                    ResponseHeader.Add(header, response.Headers.GetValues(header)?.ToList());
                response.Close();
                return html;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    var response = (HttpWebResponse)e.Response;
                    var html = GetResponseBody(response);
                    StatusCode = response.StatusCode;
                    return html;
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }


        }

        public override async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null)
        {
            Init();
            if (_request == null || string.IsNullOrEmpty(path))
                return false;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)await _request.GetResponseAsync();
                StatusCode = response.StatusCode;

            }
            catch (WebException e)
            {
                if (e.Response != null)
                    StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                return false;
            }
            catch (Exception)
            {
                throw;
            }
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                ResponseCookies = response.Cookies;
                var filesize = decimal.Parse(response.ContentLength.ToString());
                decimal percentage = 0M;
                decimal speed = 0M;
                decimal remain = 0;
                byte[] buffer = new byte[4096];
                try
                {
                    var dir = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        Directory.CreateDirectory(dir);
                    using (FileStream fs = File.Create(path))
                    {
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
                            bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            await fs.WriteAsync(buffer, 0, bytesRead);
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
                    if (File.Exists(path) && (new FileInfo(path).Length == filesize || filesize < 0))
                        return true;
                    else
                        return false;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return false;
        }

        public override async Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null)
        {
            Init();
            if (_request == null)
                return default;
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)(await _request.GetResponseAsync());
                StatusCode = response.StatusCode;
            }
            catch (Exception)
            {
                throw;
            }
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                ResponseCookies = response.Cookies;
                var FileSize = decimal.Parse(response.ContentLength.ToString());
                var filesize = decimal.Parse(response.ContentLength.ToString());
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
                        bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                        await fs.WriteAsync(buffer, 0, bytesRead);
                        if (progress != null)
                        {
                            speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                            remain = (FileSize - fs.Length) / ((speed == 0 ? 1 : speed) * 1024);
                        }
                    }
                    while (bytesRead > 0);
                    fs.Flush();
                    t.Stop();
                    progress?.Invoke(100M, 0M, 0M);
                    fs.Position = 0;
                    return fs;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return default;
        }
    }
}
