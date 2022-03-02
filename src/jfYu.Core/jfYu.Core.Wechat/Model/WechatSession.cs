using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model
{
    public class WechatSession
    {
        [JsonProperty(PropertyName = "errcode")]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "errmsg")]
        public string Msg { get; set; }

        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; }

        [JsonProperty(PropertyName = "session_key")]

        public string SessionKey { get; set; }
    }
}
