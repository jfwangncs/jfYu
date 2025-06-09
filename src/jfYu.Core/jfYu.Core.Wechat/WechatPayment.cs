using jfYu.Core.Wechat.Config;
using jfYu.Core.Wechat.Model.Payment;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace jfYu.Core.Wechat
{
    public class WechatPayment(PaymentConfig config, IHttpClientFactory httpClientFactory, ILogger<WechatPayment> logger)
    {
        private readonly ILogger<WechatPayment> _logger = logger;
        private readonly string MakeOrderUrl = "pay/unifiedorder";
        private readonly string GetOrderUrl = "pay/orderquery";
        private readonly string RefundOrderUrl = " secapi/pay/refund";
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly PaymentConfig _config = config;

        /// <summary>
        /// 统一下单
        /// </summary>
        public async Task<PaymentResult?> MakeOrderAsync(UnifiedOrder order)
        {
            _logger.LogInformation("make order start,request:{Order}", JsonConvert.SerializeObject(order));
            order.AppId = _config.AppId;
            order.MchId = _config.MchID;
            if (string.IsNullOrEmpty(order.NotifyUrl))
                order.NotifyUrl = _config.NotifyUrl;

            if (string.IsNullOrEmpty(order.SpbillCreateIp))
                throw new NullReferenceException(nameof(order.SpbillCreateIp));

            //check
            if (string.IsNullOrEmpty(order.Body))
                throw new NullReferenceException(nameof(order.Body));
            if (string.IsNullOrEmpty(order.OutTradeNo))
                throw new NullReferenceException(nameof(order.OutTradeNo));

            if (string.IsNullOrEmpty(order.TradeType))
                throw new NullReferenceException(nameof(order.TradeType));

            if (order.TradeType == "NATIVE" && string.IsNullOrEmpty(order.ProductId))
                throw new NullReferenceException($"when trade_type=NATIVE，{nameof(order.ProductId)} is mandatory");

            if (order.TradeType == "JSAPI" && string.IsNullOrEmpty(order.Openid))
                throw new NullReferenceException($"when trade_type=JSAPI，{nameof(order.Openid)} is mandatory");

            //随机数
            order.NonceStr = Guid.NewGuid().ToString("N");
            var dic = (JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(order))?.Where(q => q.Value != null)?.ToDictionary(q => q.Key, q => q.Value)) ?? throw new ArgumentNullException(nameof(order));
            order.Sign = GetSign(dic);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(order, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml")?.InnerXml ?? "";
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(MakeOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("make order successful,response:{Result}", result);
                return XmlGet<PaymentResult>(result);
            }
            _logger.LogError("make order failed,response:{Result}", result);
            return default;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult?> QueryOrderByOurOrderNoAsync(string orderNo)
        {
            _logger.LogInformation("get order by OrderNo start,orderNo:{OrderNo}", orderNo);
            var dic = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchID },
                { "out_trade_no", orderNo },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "sign_type", "MD5" }
            };
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml")?.InnerXml ?? "";
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(GetOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("get order by OrderNo successful,response:{Result}", result);
                return XmlGet<QueryOrderResult>(result);
            }
            _logger.LogError("get order by OrderNo failed,response:{Result}", result);
            return default;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="transactionId">微信订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult?> QueryOrderByWxIDAsync(string transactionId)
        {
            _logger.LogInformation("get order by WxId start,transactionId:{TransactionId}", transactionId);
            var dic = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchID },
                { "transaction_id", transactionId },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "sign_type", "MD5" }
            };
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml")?.InnerXml ?? "";
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(GetOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("get order by WxId successful,response:{Result}", result);
                return XmlGet<QueryOrderResult>(result);
            }
            _logger.LogError("get order by WxId failed,response:{Result}", result);
            return default;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="outRefundNo">退款单号</param>
        /// <param name="totalFee">总金额</param>
        /// <param name="refundFee">退款金额</param>
        /// <returns></returns>
        public async Task<RefundResult?> RefundAsync(string orderNo, string outRefundNo, int totalFee, int refundFee)
        {
            _logger.LogInformation("refund order start,orderNo:{OrderNo},outRefundNo:{OutRefundNo},totalFee：{TotalFee},refundFee:{RefundFee}", orderNo, outRefundNo, totalFee, refundFee);
            var dic = new Dictionary<string, string>
            {
                { "appid", _config.AppId },
                { "mch_id", _config.MchID },
                { "nonce_str", Guid.NewGuid().ToString("N") },
                { "sign_type", "MD5" },
                { "out_trade_no", orderNo },
                { "out_refund_no", outRefundNo },
                { "total_fee", totalFee.ToString() },
                { "refund_fee", refundFee.ToString() }
            };
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml")?.InnerXml ?? "";
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(RefundOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                _logger.LogInformation("refund order successful,response:{Result}", result);
                return XmlGet<RefundResult>(result);
            }
            _logger.LogError("refund order failed,response:{Result}", result);
            return default;
        }

        public string GetSign(Dictionary<string, string> dic)
        {
            dic = dic.OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.Value);
            var paramStr = GetParamStr(dic);
            var signTemp = $"{paramStr}&key={_config.Key}";
            return SHAmd5Encrypt(signTemp).ToUpper();
            static string GetParamStr(Dictionary<string, string> param)
            {
                string p = "";
                foreach (var item in param)
                    p += $"{item.Key}={item.Value}&";
                return p.TrimEnd('&');
            }
        }

        private static string SHAmd5Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组
            using var md5 = MD5.Create();
            var encryptbytes = md5.ComputeHash(bytes);//求哈希值
            static string BytesToHex(byte[] buffer)
            {
                string md_str = string.Empty;
                for (int i = 0; i < buffer.Length; i++)
                {
                    md_str += buffer[i].ToString("x2");
                }
                return md_str;
            }
            return BytesToHex(encryptbytes);//将Byte[]数组转为明文(其实就是字符串)
        }

        public static T? XmlGet<T>(string xml)
        {
            var obj = new JObject();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            var xmlNode = xmlDoc.FirstChild;
            var nodes = xmlNode?.ChildNodes;
            if (nodes != null)
            {
                foreach (XmlNode xn in nodes)
                {
                    var xe = (XmlElement)xn;
                    obj.Add(xe.Name, xe.InnerText);
                }
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
            }
            return default;
        }
    }
}