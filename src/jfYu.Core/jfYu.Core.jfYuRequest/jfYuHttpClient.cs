using System;
using System.IO;
using System.Net;
#if NETSTANDARD21 || NETSTANDARD20||NET5_0||NET6_0||NET7_0||NET8_0
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
#endif
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
#if NETSTANDARD21||NETSTANDARD20||NET5_0||NET6_0||NET7_0||NET8_0
    public class jfYuHttpClient : jfYuBaseRequest, IjfYuRequest
    {
        public jfYuHttpClient(string url) : base(url)
        {
        }

        private HttpClientHandler proxyHttpClientHandler;

        /// <summary>
        /// web请求
        /// </summary>
        public HttpClient Request { get; private set; }

        /// <summary>
        /// 响应信息状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {

            proxyHttpClientHandler = new HttpClientHandler() { CookieContainer = this.Cookies };
            if (Proxy != null)
            {
                //设置代理服务器IP，伪装请求地址 
                proxyHttpClientHandler.Proxy = Proxy;
                proxyHttpClientHandler.UseProxy = true;
            }            
            Request = new HttpClient(proxyHttpClientHandler);
            //_request.MaxResponseContentBufferSize = 256000; 去掉 默认2GB
            LoadHeader();
        }

        protected void LoadHeader()
        {
            try
            {
                Request.Timeout = new TimeSpan(0, 0, Timeout);  //超时设置，默认5；

                //设置请求头信息
                if (RequestHeader.Referer != "")
                    Request.DefaultRequestHeaders.Referrer = new Uri(RequestHeader.Referer);
                if (RequestHeader.Accept != "")
                    Request.DefaultRequestHeaders.Accept.ParseAdd(RequestHeader.Accept);//定义gzip压缩页面支持
                if (RequestHeader.UserAgent != "")
                    Request.DefaultRequestHeaders.UserAgent.ParseAdd(RequestHeader.UserAgent);
                if (RequestHeader.AcceptEncoding != "")
                    Request.DefaultRequestHeaders.AcceptEncoding.ParseAdd(RequestHeader.AcceptEncoding);//定义gzip压缩页面支持
                if (RequestHeader.AcceptLanguage != "")
                    Request.DefaultRequestHeaders.AcceptLanguage.ParseAdd(RequestHeader.AcceptLanguage);
                if (RequestHeader.CacheControl != "")
                    Request.DefaultRequestHeaders.Add("CacheControl", RequestHeader.CacheControl);
                if (RequestHeader.Pragma != "")
                    Request.DefaultRequestHeaders.Pragma.ParseAdd(RequestHeader.Pragma);
                if ("keep-alive".Equals(RequestHeader.Connection))
                    Request.DefaultRequestHeaders.ConnectionClose = false;
                if (RequestHeader.Host != "")
                    Request.DefaultRequestHeaders.Host = RequestHeader.Host.Replace("https://", "").Replace("http://", "");
                //添加自定义header头
                foreach (var item in CustomHeader)
                {
                    Request.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <param name="path">网页内容</param>
        public string GetHtml()
        {
            string html = "";
            for (int i = 1; i <= Repetitions; i++)
            {
                Init();
                try
                {
                    if (Method.Equals(jfYuRequestMethod.Get))
                    {
                        using var response = Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
                        StatusCode = response.StatusCode;
                        var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        html = Encoding.GetString(bytes, 0, bytes.Length - 1);
                    }
                    else
                    {
                        using var response = Request.PostAsync(Url, new StringContent(GetParaStr())).GetAwaiter().GetResult();
                        StatusCode = response.StatusCode;
                        var bytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                        html = Encoding.GetString(bytes, 0, bytes.Length - 1);
                    }
                    break;

                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,参数:{GetParaStr()},错误信息:{e.Message}", e);
                    else
                        Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,参数:{GetParaStr()},错误信息:{ex.Message}", ex);
                    else
                        Thread.Sleep(1000);
                }
            }
            return html;
        }

        /// <summary>
        /// 获取网页内容
        /// </summary>
        /// <returns>网页内容</returns>
        public async Task<string> SendAsync()
        {
            string html = "";
            for (int i = 1; i <= Repetitions; i++)
            {
                Init();
                try
                {
                    if (Method.Equals(jfYuRequestMethod.Get))
                    {

                        using var response = await Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead);
                        StatusCode = response.StatusCode;
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        html = Encoding.GetString(bytes, 0, bytes.Length - 1);

                    }
                    else
                    {
                        using var response = await Request.PostAsync(Url, new StringContent(GetParaStr()));
                        StatusCode = response.StatusCode;
                        var bytes = await response.Content.ReadAsByteArrayAsync();
                        html = Encoding.GetString(bytes, 0, bytes.Length - 1);
                    }
                    break;
                }
                catch (WebException e)
                {
                    if (e.Response != null)
                        StatusCode = ((HttpWebResponse)e.Response).StatusCode;
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,参数:{GetParaStr()},错误信息:{e.Message}", e);
                    else
                        await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    if (i >= Repetitions)
                        throw new Exception($"重复请求失败,参数:{GetParaStr()},错误信息:{ex.Message}", ex);
                    else
                        await Task.Delay(5000);
                }

            }
            return html;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">保存地址</param>
        public bool GetFile(string path, Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();

            using var response = Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                //文件大小
                var FileSize = decimal.Parse(response.Content.Headers.ContentLength.ToString());
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
                    using FileStream fs = File.Create(path);
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
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }

                if (File.Exists(path) && (new FileInfo(path).Length == FileSize || FileSize < 0))
                    return true;
                else
                    return false;
            }
            else
                return false;

        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="path">保存地址</param>
        public async Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            using var response = await Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead);
            StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                //文件大小
                var FileSize = decimal.Parse(response.Content.Headers.ContentLength.ToString());
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
                    using FileStream fs = File.Create(path);
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
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }

                if (File.Exists(path) && (new FileInfo(path).Length == FileSize || FileSize < 0))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public MemoryStream GetFile(Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            using var response = Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                //文件大小
                var FileSize = decimal.Parse(response.Content.Headers.ContentLength.ToString());
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
                    return fs;
                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else
                return null;
        }

        public async Task<MemoryStream> GetFileAsync(Action<decimal, decimal, decimal> setProgress = null)
        {
            Init();
            using var response = await Request.GetAsync(GetParaStr() == "" ? Url : Url + "?" + GetParaStr(), HttpCompletionOption.ResponseHeadersRead);
            StatusCode = response.StatusCode;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using Stream responseStream = await response.Content.ReadAsStreamAsync();
                //文件大小
                var FileSize = decimal.Parse(response.Content.Headers.ContentLength.ToString());
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
                    return fs;
                }
                catch (Exception ex)
                {
                    throw new Exception("下载文件出错", ex);
                }
            }
            else
                return null;
        }
    }
#endif
}
