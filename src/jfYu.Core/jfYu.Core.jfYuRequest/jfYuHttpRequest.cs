using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
    public class jfYuHttpRequest : jfYuBaseRequest, IjfYuRequest
    {
        public jfYuHttpRequest(string url) : base(url)
        {
        }

        /// <summary>
        /// web请求
        /// </summary>
        public HttpWebRequest Request { get; private set; }

        /// <summary>
        /// 响应信息状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (Method.Equals(jfYuRequestMethod.Get))
            {
                Request = (HttpWebRequest)WebRequest.Create(GetParaStr() == "" ? Url : Url + "?" + GetParaStr());
                Request.Method = "GET";
                Request.ContentType = ContentType == "" ? jfYuRequestContentType.TextHtml : ContentType;
                Request.CookieContainer = this.Cookies;//加入cookie
            }
            else
            {
                Request = (HttpWebRequest)WebRequest.Create(Url);
                Request.Method = "POST";
                if (UsePayload)
                {
                    var memStream = new MemoryStream();
                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    string Enter = "\r\n";
                    foreach (var item in Para)
                    {
                        string pstr = Enter + "--" + boundary + Enter
                      + "Content-Disposition: form-data; name=\"" + item.Key + "\"" + Enter + Enter
                      + item.Value;
                        var StrByte = Encoding.GetBytes(pstr);//所有字符串二进制
                        memStream.Write(StrByte, 0, StrByte.Length);
                    }
                    //文件上传拼接  
                    foreach (var file in Files)
                    {
                        FileStream fs = new FileStream(file.Value, FileMode.Open, FileAccess.Read);
                        var fileContentByte = new byte[fs.Length]; // 二进制文件
                        fs.Read(fileContentByte, 0, Convert.ToInt32(fs.Length));
                        fs.Close();

                        string fileContentStr = Enter + "--" + boundary + Enter
                       + $"Content-Disposition: form-data; name=\"{file.Key}\"; filename=\"{Path.GetFileName(file.Value)}\""
                       + Enter + "Content-Type:application/octet-stream" + Enter + Enter;
                        var fileContentStrByte = Encoding.UTF8.GetBytes(fileContentStr);//fileContent一些名称等信息的二进制（不包含文件本身）
                        memStream.Write(fileContentStrByte, 0, fileContentStrByte.Length);
                        memStream.Write(fileContentByte, 0, fileContentByte.Length);

                    }
                    var endBoundary = Encoding.UTF8.GetBytes(Enter + "--" + boundary + "--" + Enter);
                    memStream.Write(endBoundary, 0, endBoundary.Length);
                    memStream.Position = 0;
                    var tempBuffer = new byte[memStream.Length];
                    memStream.Read(tempBuffer, 0, tempBuffer.Length);
                    memStream.Close();
                    Request.ContentLength = tempBuffer.Length;
                    Request.ContentType = "multipart/form-data;boundary=" + boundary;
                    using Stream reqStream = Request.GetRequestStream();
                    reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                    reqStream.Close();
                }
                else
                {
                    Request.ContentType = ContentType == "" ? jfYuRequestContentType.XWWWFormUrlEncoded : ContentType;
                    byte[] tempBuffer = Encoding.GetBytes(GetParaStr());
                    Request.ContentLength = tempBuffer.Length;
                    using Stream reqStream = Request.GetRequestStream();
                    reqStream.Write(tempBuffer, 0, tempBuffer.Length);
                    reqStream.Close();
                }
            }
            Request.CookieContainer = this.Cookies;//加入cookie
            if (Cert != null)
                Request.ClientCertificates.Add(Cert);
            LoadHeader();
        }
        /// <summary>
        /// 加载header头
        /// </summary>
        protected void LoadHeader()
        {
            try
            {
                Request.Timeout = Timeout * 1000;//超时设置，默认5；
                                                 //设置请求头信息
                Request.UserAgent = RequestHeader.UserAgent;
                Request.Headers.Add(HttpRequestHeader.AcceptEncoding, RequestHeader.AcceptEncoding);//定义gzip压缩页面支持
                Request.Headers.Add(HttpRequestHeader.AcceptLanguage, RequestHeader.AcceptLanguage);
                Request.Headers.Add(HttpRequestHeader.CacheControl, RequestHeader.CacheControl);
                Request.Headers.Add(HttpRequestHeader.Pragma, RequestHeader.Pragma);
                if ("keep-alive".Equals(RequestHeader.Connection))
                    Request.KeepAlive = true;//启用长连接  
                if (RequestHeader.Host != "")
                    Request.Host = RequestHeader.Host.Replace("https://", "").Replace("http://", "");
                Request.Referer = RequestHeader.Referer;
                Request.Accept = RequestHeader.Accept;
                if (Proxy != null)
                    Request.Proxy = Proxy;//设置代理服务器IP，伪装请求地址
                //添加自定义header头
                foreach (var item in CustomHeader)
                {
                    Request.Headers.Add(item.Key, item.Value);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        /// <summary>
        /// 解析数据
        /// </summary>       
        protected string GetResponseBody(HttpWebResponse response)
        {
            string responseBody = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(response.ContentEncoding))
                {
                    if (response.ContentEncoding.ToLower().Contains("gzip"))
                    {
                        using GZipStream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using StreamReader reader = new StreamReader(stream, this.Encoding);
                        responseBody = reader.ReadToEnd();
                        responseBody = responseBody.Replace("\0", "");
                    }
                    else if (response.ContentEncoding.ToLower().Contains("deflate"))
                    {
                        using DeflateStream stream = new DeflateStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using StreamReader reader = new StreamReader(stream, this.Encoding);
                        responseBody = reader.ReadToEnd();
                        responseBody = responseBody.Replace("\0", "");
                    }
                    else if (response.ContentEncoding.ToLower().Contains("br") || response.ContentEncoding.ToLower().Contains("brotli"))
                    {
                        using Brotli.BrotliStream stream = new Brotli.BrotliStream(response.GetResponseStream(), CompressionMode.Decompress);
                        using StreamReader reader = new StreamReader(stream, this.Encoding);
                        responseBody = reader.ReadToEnd();
                        responseBody = responseBody.Replace("\0", "");
                    }
                }
                else
                {
                    using Stream stream = response.GetResponseStream();
                    using StreamReader reader = new StreamReader(stream, this.Encoding);
                    responseBody = reader.ReadToEnd();
                    responseBody = responseBody.Replace("\0", "");
                }

            }
            catch (Exception)
            {
                //解决莫名的bug（多if直接报错）
                using Stream stream = response.GetResponseStream();
                using StreamReader reader = new StreamReader(stream, this.Encoding);
                responseBody = reader.ReadToEnd();
                responseBody = responseBody.Replace("\0", "");
            }
            return responseBody;
        }

        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <returns>网页内容</returns>
        public string GetHtml()
        {
            string html = "";
            for (int i = 1; i <= Repetitions; i++)
            {
                Init();
                try
                {
                    using var response = (HttpWebResponse)Request.GetResponse();
                    StatusCode = response.StatusCode;
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                        html = GetResponseBody(response);
                    else if (response.StatusCode == HttpStatusCode.Moved)
                    {
                        string newUrl = response.Headers.GetValues("Location")[0];
                        jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                        SetCookies = x.SetCookies;
                        return x.GetHtml();
                    }
                    else
                        return "";
                    SetCookies = response.Cookies;
                    response.Close();
                    break;
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,地址:{Request.RequestUri},参数:{GetParaStr()},错误信息:{e.Message}", e);
                    else
                        Task.Delay(5000).Wait();
                }
                catch (Exception ex)
                {
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,地址:{Request.RequestUri},参数:{GetParaStr()},错误信息:{ex.Message}", ex);
                    else
                        Task.Delay(5000).Wait();
                }
            }
            return html;
        }

        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <returns>网页内容</returns>
        public async Task<string> GetHtmlAsync()
        {
            string html = "";
            for (int i = 1; i <= Repetitions; i++)
            {
                Init();
                try
                {
                    using var response = (HttpWebResponse)await Request.GetResponseAsync();
                    StatusCode = response.StatusCode;
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                        html = GetResponseBody(response);
                    else if (response.StatusCode == HttpStatusCode.Moved)
                    {
                        string newUrl = response.Headers.GetValues("Location")[0];
                        jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                        SetCookies = x.SetCookies;
                        return await x.GetHtmlAsync();
                    }
                    else
                        return "";
                    SetCookies = response.Cookies;
                    response.Close();
                    break;

                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,地址:{Request.RequestUri},参数:{GetParaStr()},错误信息:{e.Message}", e);
                    else
                        await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,地址:{Request.RequestUri},参数:{GetParaStr()},错误信息:{ex.Message}", ex);
                    else
                        await Task.Delay(5000);
                }
            }
            return html;
        }

        public bool GetFile(string path, Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)Request.GetResponse();
                StatusCode = response.StatusCode;
            }
            catch (WebException e)
            {
                StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                return false;
            }

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                SetCookies = response.Cookies;
                //文件大小
                var FileSize = decimal.Parse(response.ContentLength.ToString());
                //下载进度
                decimal Progress = 0M;
                //下载速度KB/s
                decimal Speed = 0M;
                //剩余时间/秒
                decimal Remain = 0;
                //下载文件块大小，后期可通过此次进行速度限制
                byte[] buffer = new byte[4096];
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (FileStream fs = File.Create(path))
                    {
                        //开启计时
                        long LastSaveSize = 0;
                        var t = new System.Timers.Timer(1000)
                        {
                            AutoReset = true,
                            Enabled = true
                        };
                        t.Elapsed += (s, e) =>
                        {
                            LastSaveSize = fs.Length;
                            setProgress?.Invoke(Progress, Speed, Remain);
                        };
                        t.Start();
                        int bytesRead;
                        do
                        {
                            bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, bytesRead);
                            if (setProgress != null)
                            {
                                //计算进度
                                Progress = fs.Length / FileSize * 100;
                                //计算速度
                                Speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                                //剩余时间
                                Remain = (FileSize - fs.Length) / ((Speed == 0 ? 1 : Speed) * 1024);
                            }
                        }
                        while (bytesRead > 0);
                        fs.Flush();
                        t.Stop();
                        setProgress?.Invoke(100M, 0M, 0M);
                    }
                    if (File.Exists(path) && (new FileInfo(path).Length == FileSize || FileSize < 0))
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Moved)
            {
                string newUrl = response.Headers.GetValues("Location")[0];
                jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                return x.GetFile(path, setProgress);
            }
            else
                return false;
        }

        public async Task<bool> GetFileAsync(string path, Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)(await Request.GetResponseAsync());
                StatusCode = response.StatusCode;

            }
            catch (WebException e)
            {
                StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                return false;
            }
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                SetCookies = response.Cookies;
                //文件大小
                var FileSize = decimal.Parse(response.ContentLength.ToString());
                //下载进度
                decimal Progress = 0M;
                //下载速度KB/s
                decimal Speed = 0M;
                //剩余时间/秒
                decimal Remain = 0;
                //下载文件块大小，后期可通过此次进行速度限制
                byte[] buffer = new byte[4096];
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    using (FileStream fs = File.Create(path))
                    {
                        //开启计时
                        long LastSaveSize = 0;
                        var t = new System.Timers.Timer(1000)
                        {
                            AutoReset = true,
                            Enabled = true
                        };
                        t.Elapsed += (s, e) =>
                        {
                            LastSaveSize = fs.Length;
                            setProgress?.Invoke(Progress, Speed, Remain);
                        };
                        t.Start();
                        int bytesRead;
                        do
                        {
                            bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                            await fs.WriteAsync(buffer, 0, bytesRead);
                            if (setProgress != null)
                            {
                                //计算进度
                                Progress = fs.Length / FileSize * 100;
                                //计算速度
                                Speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                                //剩余时间
                                Remain = (FileSize - fs.Length) / ((Speed == 0 ? 1 : Speed) * 1024);
                            }
                        }
                        while (bytesRead > 0);
                        fs.Flush();
                        t.Stop();
                        setProgress?.Invoke(100M, 0M, 0M);
                    }
                    if (File.Exists(path) && (new FileInfo(path).Length == FileSize || FileSize < 0))
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Moved)
            {
                string newUrl = response.Headers.GetValues("Location")[0];
                jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                return await x.GetFileAsync(path, setProgress);
            }
            else
                return false;
        }

        public MemoryStream GetFile(Action<decimal, decimal, decimal> setProgress = null)
        {

            Init();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)Request.GetResponse();
                StatusCode = response.StatusCode;
            }
            catch (WebException e)
            {
                StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                return null;
            }

            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                SetCookies = response.Cookies;
                //文件大小
                var FileSize = decimal.Parse(response.ContentLength.ToString());
                //下载进度
                decimal Progress = 0M;
                //下载速度KB/s
                decimal Speed = 0M;
                //剩余时间/秒
                decimal Remain = 0;
                //下载文件块大小，后期可通过此次进行速度限制
                byte[] buffer = new byte[4096];
                try
                {
                    MemoryStream fs = new MemoryStream();
                    //开启计时
                    long LastSaveSize = 0;
                    var t = new System.Timers.Timer(1000)
                    {
                        AutoReset = true,
                        Enabled = true
                    };
                    t.Elapsed += (s, e) =>
                    {
                        LastSaveSize = fs.Length;
                        setProgress?.Invoke(Progress, Speed, Remain);
                    };
                    t.Start();
                    int bytesRead;
                    do
                    {
                        bytesRead = responseStream.Read(buffer, 0, buffer.Length);
                        fs.Write(buffer, 0, bytesRead);
                        if (setProgress != null)
                        {
                            //计算进度
                            Progress = fs.Length / FileSize * 100;
                            //计算速度
                            Speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                            //剩余时间
                            Remain = (FileSize - fs.Length) / ((Speed == 0 ? 1 : Speed) * 1024);
                        }
                    }
                    while (bytesRead > 0);
                    fs.Flush();
                    t.Stop();
                    setProgress?.Invoke(100M, 0M, 0M);
                    fs.Position = 0;
                    return fs;

                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Moved)
            {
                string newUrl = response.Headers.GetValues("Location")[0];
                jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                return x.GetFile(setProgress);
            }
            else
                return null;
        }

        public async Task<MemoryStream> GetFileAsync(Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)(await Request.GetResponseAsync());
                StatusCode = response.StatusCode;

            }
            catch (WebException e)
            {
                StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                return null;
            }
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.GetResponseStream();
                SetCookies = response.Cookies;
                //文件大小
                var FileSize = decimal.Parse(response.ContentLength.ToString());
                //下载进度
                decimal Progress = 0M;
                //下载速度KB/s
                decimal Speed = 0M;
                //剩余时间/秒
                decimal Remain = 0;
                //下载文件块大小，后期可通过此次进行速度限制
                byte[] buffer = new byte[4096];
                try
                {
                    MemoryStream fs = new MemoryStream();
                    //开启计时
                    long LastSaveSize = 0;
                    var t = new System.Timers.Timer(1000)
                    {
                        AutoReset = true,
                        Enabled = true
                    };
                    t.Elapsed += (s, e) =>
                    {
                        LastSaveSize = fs.Length;
                        setProgress?.Invoke(Progress, Speed, Remain);
                    };
                    t.Start();
                    int bytesRead;
                    do
                    {
                        bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
                        await fs.WriteAsync(buffer, 0, bytesRead);
                        if (setProgress != null)
                        {
                            //计算进度
                            Progress = fs.Length / FileSize * 100;
                            //计算速度
                            Speed = (decimal)(fs.Length - LastSaveSize) / 1024;
                            //剩余时间
                            Remain = (FileSize - fs.Length) / ((Speed == 0 ? 1 : Speed) * 1024);
                        }
                    }
                    while (bytesRead > 0);
                    fs.Flush();
                    t.Stop();
                    setProgress?.Invoke(100M, 0M, 0M);
                    fs.Position = 0;
                    return fs;
                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else if (response.StatusCode == HttpStatusCode.Moved)
            {
                string newUrl = response.Headers.GetValues("Location")[0];
                jfYuHttpRequest x = new jfYuHttpRequest($"{newUrl}");
                return await x.GetFileAsync(setProgress);
            }
            else
                return null;
        }
    }
}
