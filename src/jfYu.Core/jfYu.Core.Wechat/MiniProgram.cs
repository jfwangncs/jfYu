using jfYu.Core.Wechat.Config;
using jfYu.Core.Wechat.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace jfYu.Core.Wechat
{
    public class MiniProgram(WechatConfig config, IHttpClientFactory httpClientFactory, ILogger<MiniProgram> logger)
    {
        private readonly string LoginUrl = "sns/jscode2session";
        private readonly string GetTokenUrl = "cgi-bin/token";
        private readonly string GetPhonenUrl = "wxa/business/getuserphonenumber";
        private readonly ILogger<MiniProgram> _logger = logger;
        private readonly WechatConfig _config = config;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        /// <summary>
        /// 获取sessionid openid
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<WechatSession?> LoginAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException(nameof(code));

            try
            {
                _logger.LogInformation("login start,code:{Code}", code);
                var httpClient = _httpClientFactory.CreateClient(Constant.Mini);
                var httpResponseMessage = await httpClient.GetAsync($"{LoginUrl}?appid={_config.AppId}&secret={_config.Secret}&js_code={code}&grant_type=authorization_code");
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<WechatSession>(result);
                _logger.LogError("login failed,code:{Code},response:{Result},status:{HttpResponseMessageStatusCode}", code, result, httpResponseMessage.StatusCode);
                return new WechatSession() { Code = ErrorCode.Failed };
            }
            catch (Exception ex)
            {
                _logger.LogError("login failed,code:{code},error:{Message}", code, ex.Message);
                return new WechatSession() { Code = ErrorCode.Failed, Msg = ex.Message };
            }
        }

        /// <summary>
        /// 获取小程序全局唯一后台接口调用凭据
        /// </summary>
        /// <returns></returns>
        public async Task<AccessToken?> GetTokenAsync()
        {
            try
            {
                _logger.LogInformation("get token start");
                var httpClient = _httpClientFactory.CreateClient(Constant.Mini);
                var httpResponseMessage = await httpClient.GetAsync($"{GetTokenUrl}?appid={_config.AppId}&secret={_config.Secret}&grant_type=client_credential");

                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<AccessToken>(result);
                _logger.LogError("get token failed,response:{Result},status:{HttpResponseMessageStatusCode}", result, httpResponseMessage.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("get token failed,error:{Message}", ex.Message + ex?.InnerException?.Message);
                return default;
            }
        }

        /// <summary>
        /// code换取用户手机号。 每个code只能使用一次，code的有效期为5min
        /// </summary>
        /// <param name="code">手机号获取凭证</param>
        /// <returns></returns>

        public async Task<WechatResult<PhoneInfo>?> GetPhoneAsync(string code)
        {

            try
            {
                _logger.LogInformation("get phone start");
                var token = await GetTokenAsync();
                var content = new StringContent(JsonConvert.SerializeObject(new { code }), Encoding.UTF8, "application/json");

                var httpClient = _httpClientFactory.CreateClient(Constant.Mini);
                var httpResponseMessage = await httpClient.PostAsync($"{GetPhonenUrl}?access_token={token?.Token}", content);
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<WechatResult<PhoneInfo>>(result);

                _logger.LogError("get phone failed,response:{Result},status:{HttpResponseMessageStatusCode}", result, httpResponseMessage.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("get phone failed,error:{Message}", ex.Message + ex?.InnerException?.Message);
                return default;
            }
        }
    }
}