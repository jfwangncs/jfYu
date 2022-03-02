using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model
{
    public class PhoneInfo
    {
        [JsonProperty(PropertyName = "phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "purePhoneNumber")]
        public string PurePhoneNumber { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty(PropertyName = "watermark")]
        public Watermark Watermark { get; set; }
    }

    public class Watermark
    {
        [JsonProperty(PropertyName = "appid")]
        public string Appid { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

    }
}
