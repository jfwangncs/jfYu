using jfYu.Core.Common.Utilities;
using jfYu.Core.Configuration;
using jfYu.Core.Wechat.Model.Payment;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace jfYu.Core.Wechat
{
    public class WechatPayment
    {
        public PaymentConfig Config { get; }
        public WechatPayment()
        {
            //读取配置文件
            try
            {
                Config = AppConfig.Configuration?.GetSection("WechatPayment")?.Get<PaymentConfig>();
            }
            catch (Exception ex)
            {
                throw new Exception("读取配置文件出错", ex);
            }
        }
        public WechatPayment(PaymentConfig config)
        {
            if (config == null)
                throw new Exception("配置为空");
            Config = config;
        }


        /// <summary>
        /// 统一下单
        /// </summary>
        public void Unify(UnifiedOrder order)
        {
            order.AppId = Config.AppId;
            order.MchId = Config.MchID;
            if (string.IsNullOrEmpty(order.NotifyUrl))
                order.NotifyUrl = Config.NotifyUrl;

            if (string.IsNullOrEmpty(order.SpbillCreateIp))
                throw new ArgumentNullException($"参数{nameof(order.SpbillCreateIp)}必须有值");
            //check
 
            if (string.IsNullOrEmpty(order.Body))
                throw new ArgumentNullException($"参数{nameof(order.Body)}必须有值");
            if (string.IsNullOrEmpty(order.OutTradeNo))
                throw new ArgumentNullException($"参数{nameof(order.OutTradeNo)}必须有值");

            if (string.IsNullOrEmpty(order.TradeType))
                throw new ArgumentNullException($"参数{nameof(order.TradeType)}必须有值");

            if (order.TradeType == "NATIVE" && string.IsNullOrEmpty(order.ProductId))

                throw new ArgumentNullException($"trade_type=NATIVE时，参数{nameof(order.ProductId)}必须有值");

            if (order.TradeType == "JSAPI" && string.IsNullOrEmpty(order.Openid))
                throw new ArgumentNullException($"trade_type=JSAPI时，参数{nameof(order.Openid)}必须有值");

            //随机数
            order.NonceStr = Guid.NewGuid().ToString("N");
            order.Sign = GetSign(order);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(order), "xml").InnerXml;
        }

        private string GetSign(UnifiedOrder order)
        {
            var json = JsonConvert.SerializeObject(order);
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json).OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.Value);
            var paramStr = GetParamStr(dic);
            var signTemp = $"{paramStr}&key={Config.Key}";
            return signTemp.SHAmd5Encrypt().ToUpper();
            string GetParamStr(Dictionary<string, string> param)
            {
                string p = "";
                foreach (var item in param)
                    p += $"{item.Key}={item.Value}&";
                p += param;
                return p;
            }


        }
    }
}
