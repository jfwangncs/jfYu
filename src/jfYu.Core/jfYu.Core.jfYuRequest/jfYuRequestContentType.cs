namespace jfYu.Core.jfYuRequest
{
    public class jfYuRequestContentType
    {
        public static string XWWWFormUrlEncoded { get; private set; } = "application/x-www-form-urlencoded";
        public static string FormData { get; private set; } = "multipart/form-data";
        public static string Json { get; private set; } = "application/json";
        public static string TextHtml { get; private set; } = "text/xml";
    }

}
