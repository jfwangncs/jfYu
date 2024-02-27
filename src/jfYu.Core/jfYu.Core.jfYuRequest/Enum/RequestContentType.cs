 
namespace jfYu.Core.jfYuRequest.Enum
{
    /// <summary>
    /// request header default values
    /// </summary>
    public class RequestContentType
    {
        public static string TextHtml { get; set; } = "text/html";
        public static string TextPlain { get; set; } = "text/plain";
        public static string TextXml { get; set; } = "text/xml";
        public static string ImageGif { get; set; } = "image/gif";
        public static string ImageJpeg { get; set; } = "image/jpeg";
        public static string ImagePng { get; set; } = "image/png";
        public static string Xml { get; set; } = "application/xml";
        public static string Json { get; set; } = "application/json";
        public static string XWWWFormUrlEncoded { get; set; } = "application/x-www-form-urlencoded";
        public static string FormData { get; set; } = "multipart/form-data";
    }
}
