using jfYu.Core.Configuration;
using jfYu.Core.jfYuRequest;
using jfYu.Core.Wechat.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;

namespace jfYu.Core.Wechat
{
    public class MiniProgram
    {
        public WechatConfig Config { get; }
        public MiniProgram()
        {
            //读取配置文件
            try
            {
                Config = AppConfig.Configuration?.GetSection("Wechat")?.Get<WechatConfig>();
            }
            catch (Exception ex)
            {
                throw new Exception("读取配置文件出错", ex);
            }
        }
        public MiniProgram(WechatConfig config)
        {
            if (config == null)
                throw new Exception("配置为空");
            Config = config;
        }

        #region 方法

        /// <summary>
        /// 获取sessionid openid
        /// </summary>
        /// <param name="code">code</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public WechatSession Auth(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            try
            {
                var res = new jfYuHttpRequest("https://api.weixin.qq.com/sns/jscode2session");
                res.RawPara = $"appid={Config.AppId}&secret={Config.Secret}&js_code={code}&grant_type=authorization_code";
                return JsonConvert.DeserializeObject<WechatSession>(res.GetHtml());
            }
            catch (Exception ex)
            {
                return new WechatSession() { Code = "-1", Msg = ex.Message };
            }
        }

        /// <summary>
        /// 获取小程序全局唯一后台接口调用凭据
        /// </summary>
        /// <returns></returns>
        public AccessToken GetToken()
        {

            try
            {
                var res = new jfYuHttpRequest("https://api.weixin.qq.com/cgi-bin/token");
                res.RawPara = $"appid={Config.AppId}&secret={Config.Secret}&grant_type=client_credential";
                return JsonConvert.DeserializeObject<AccessToken>(res.GetHtml());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// code换取用户手机号。 每个code只能使用一次，code的有效期为5min
        /// </summary>
        /// <param name="code">手机号获取凭证</param>
        /// <returns></returns>

        public WechatResult<PhoneInfo> GetPhone(string code)
        {
            var token = GetToken();
            try
            {
                var res = new jfYuHttpRequest($"https://api.weixin.qq.com/wxa/business/getuserphonenumber?access_token={token.Token}");
                res.Para.Add("code", code);
                res.Method = jfYuRequestMethod.Post;
                res.ContentType = "application/json";
                return JsonConvert.DeserializeObject<WechatResult<PhoneInfo>>(res.GetHtml());
            }
            catch (Exception)
            {

                throw;
            }

        }
        #endregion
    }
}