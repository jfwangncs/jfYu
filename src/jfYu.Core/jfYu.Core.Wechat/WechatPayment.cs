using jfYu.Core.Common.Utilities;
using jfYu.Core.Configuration;
using jfYu.Core.jfYuRequest;
using jfYu.Core.Wechat.Model.Payment;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Xml;

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
        public async Task<PaymentResult> UnifyAsync(UnifiedOrder order)
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
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(order))
                .Where(q => q.Value != null).ToDictionary(q => q.Key, q => q.Value);
            order.Sign = GetSign(dic);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(order, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var res = new jfYuHttpRequest("https://api.mch.weixin.qq.com/pay/unifiedorder");
            res.Method = jfYuRequestMethod.Post;
            res.RawPara = xml;
            var html = await res.GetHtmlAsync();
            var obj = new JObject();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(html);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement)xn;
                obj.Add(xe.Name, xe.InnerText);
            }
            return JsonConvert.DeserializeObject<PaymentResult>(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult> QueryOrderByOurOrderNoAsync(string orderNo)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", Config.AppId);
            dic.Add("mch_id", Config.MchID);
            dic.Add("out_trade_no", orderNo);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var res = new jfYuHttpRequest("https://api.mch.weixin.qq.com/pay/orderquery");
            res.Method = jfYuRequestMethod.Post;
            res.RawPara = xml;
            var html = await res.GetHtmlAsync();
            var obj = new JObject();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(html);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement)xn;
                obj.Add(xe.Name, xe.InnerText);
            }
            return JsonConvert.DeserializeObject<QueryOrderResult>(JsonConvert.SerializeObject(obj));
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="transactionId">微信订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult> QueryOrderByWxIDAsync(string transactionId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", Config.AppId);
            dic.Add("mch_id", Config.MchID);
            dic.Add("transaction_id", transactionId);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var res = new jfYuHttpRequest("https://api.mch.weixin.qq.com/pay/orderquery");
            res.Method = jfYuRequestMethod.Post;
            res.RawPara = xml;
            var html = await res.GetHtmlAsync();
            var obj = new JObject();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(html);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement)xn;
                obj.Add(xe.Name, xe.InnerText);
            }
            return JsonConvert.DeserializeObject<QueryOrderResult>(JsonConvert.SerializeObject(obj));
        }

        public async Task<NotifyResult> NotifyCheck(string xml)
        {
            var obj = new JObject();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement)xn;
                obj.Add(xe.Name, xe.InnerText);
            }
            return JsonConvert.DeserializeObject<NotifyResult>(JsonConvert.SerializeObject(obj));           

        }
        public async Task<RefundResult> RefundAsync(string orderNo, string outRefundNo, int totalFee, int refundFee)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", Config.AppId);
            dic.Add("mch_id", Config.MchID);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            dic.Add("out_trade_no", orderNo);
            dic.Add("out_refund_no", outRefundNo);
            dic.Add("total_fee", totalFee.ToString());
            dic.Add("refund_fee", refundFee.ToString());
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var res = new jfYuHttpRequest("https://api.mch.weixin.qq.com/secapi/pay/refund");
            res.Method = jfYuRequestMethod.Post;
            res.RawPara = xml;
            X509Certificate2 cert = new X509Certificate2(AppContext.BaseDirectory + Config.CertPath, Config.MchID);
            res.Cert = cert;
            var html = await res.GetHtmlAsync();
            var obj = new JObject();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(html);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode.ChildNodes;
            foreach (XmlNode xn in nodes)
            {
                var xe = (XmlElement)xn;
                obj.Add(xe.Name, xe.InnerText);
            }
            return JsonConvert.DeserializeObject<RefundResult>(JsonConvert.SerializeObject(obj));

        }

        public string GetSign(Dictionary<string, string> dic)
        {
            dic = dic.OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.Value);
            var paramStr = GetParamStr(dic);
            var signTemp = $"{paramStr}&key={Config.Key}";
            return signTemp.SHAmd5Encrypt().ToUpper();
            string GetParamStr(Dictionary<string, string> param)
            {
                string p = "";
                foreach (var item in param)
                    p += $"{item.Key}={item.Value}&";
                return p.TrimEnd('&');
            }

        }
    }
}
