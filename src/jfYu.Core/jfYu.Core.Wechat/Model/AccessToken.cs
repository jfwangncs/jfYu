using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model
{
    public class AccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Token { get; set; } = null!;

        [JsonProperty(PropertyName = "expires_in")]
        public int Expires { get; set; }
    }
}