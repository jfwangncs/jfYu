using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model
{
    public class WechatResult<T> where T : class
    {
        [JsonProperty(PropertyName = "errcode")]
        public string ErrCode { get; set; }

        [JsonProperty(PropertyName = "errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty(PropertyName = "phone_info")]
        public T PhoneInfo { get; set; }
    }
}
