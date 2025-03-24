
namespace jfYu.Core.jfYuRequest.Enum
{
    /// <summary>
    /// request header default values
    /// </summary>
    public class RequestContentType
    {
        public static string Plain { get; } = "text/plain";
        public static string Xml { get; } = "application/xml";
        public static string Json { get; } = "application/json";
        public static string FormUrlEncoded { get; } = "application/x-www-form-urlencoded";
        public static string FormData { get; } = "multipart/form-data";
    }
}
