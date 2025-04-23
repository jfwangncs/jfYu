using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model
{
    public class PhoneInfo
    {
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [JsonProperty(PropertyName = "purePhoneNumber")]
        public string PurePhoneNumber { get; set; } = null!;

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; } = null!;

        [JsonProperty(PropertyName = "watermark")]
        public Watermark Watermark { get; set; } = null!;
    }

    public class Watermark
    {
        [JsonProperty(PropertyName = "appid")]
        public string Appid { get; set; } = null!;

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }
    }
}