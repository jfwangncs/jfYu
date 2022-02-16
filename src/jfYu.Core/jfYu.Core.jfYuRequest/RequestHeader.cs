namespace jfYu.Core.jfYuRequest
{
    public class RequestHeader
    {
        public string Accept { get; set; } = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        public string AcceptEncoding { get; set; } = "gzip, deflate, br";
        public string AcceptLanguage { get; set; } = "zh-CN,zh;q=0.9,en;q=0.8";
        public string CacheControl { get; set; } = "no-cache";
        public string Connection { get; set; } = "keep-alive";
        public string Host { get; set; } = "";
        public string Pragma { get; set; } = "no-cache";
        public string Referer { get; set; } = "";
        public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36";
    }



}
