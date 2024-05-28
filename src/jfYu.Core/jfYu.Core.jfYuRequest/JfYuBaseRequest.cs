using jfYu.Core.jfYuRequest.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.jfYuRequest
{
    public abstract class JfYuBaseRequest : IJfYuRequest
    {

        public string Url { get; set; } = "";
        public string ContentType { get; set; } = RequestContentType.TextHtml;
        public RequestMethod Method { get; set; } = RequestMethod.Get;
        public Dictionary<string, string> Params { get; set; } = [];
        public string RawParams { get; set; } = "";
        public string Authorization { get; set; } = "";  
        public Encoding RequestEncoding { get; set; } = Encoding.UTF8;
        public CookieContainer RequestCookies { get; set; } = new();
        public CookieCollection ResponseCookies { get; set; } = [];
        public WebProxy? Proxy { get; set; }
        public Dictionary<string, string> Files { get; set; } = [];
        public RequestHeader RequestHeader { get; set; } = new();
        public int Timeout { get; set; } = 5;
        public bool UsePayload { get; set; } = false;
        public Dictionary<string, string> RequestCustomHeaders { get; set; } = [];
        public X509Certificate2? Cert { get; set; }
        public bool CertificateValidation { get; set; } = false;
        public HttpStatusCode StatusCode { get; protected set; }
        public Action<object>? CustomInitFunc { get; set; }
        public Dictionary<string, List<string>?> ResponseHeader { get; protected set; } = new Dictionary<string, List<string>?>();

        protected string GetParamString()
        {
            try
            {
                if (ContentType == RequestContentType.Json)
                {
                    if (!string.IsNullOrEmpty(RawParams))
                    {
                        var RawParaDic = RawParams.Split('&');
                        foreach (var item in RawParaDic)
                        {
                            var r = item.Split('=');
                            Params.Add(r[0], r[1]);
                        }
                    }
                    return JsonConvert.SerializeObject(Params);
                }
                else
                {
                    string p = "";
                    foreach (var item in Params)
                        p += $"{item.Key}={item.Value}&";
                    p = p.Trim('&') + RawParams;
                    return p;
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public abstract Task<string> SendAsync();

        public abstract Task<bool> DownloadFileAsync(string path, Action<decimal, decimal, decimal>? progress = null);

        public abstract Task<MemoryStream?> DownloadFileAsync(Action<decimal, decimal, decimal>? progress = null);
                
    }
}
