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
    public class WechatPayment
    {
        private readonly ILogger<WechatPayment> _logger;
        private readonly string MakeOrderUrl = "pay/unifiedorder";
        private readonly string GetOrderUrl = "pay/orderquery";
        private readonly string RefundOrderUrl = " secapi/pay/refund";
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly PaymentConfig _config;
        public WechatPayment(PaymentConfig config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 统一下单
        /// </summary>
        public async Task<PaymentResult> MakeOrderAsync(UnifiedOrder order)
        {
            _logger.LogInformation($"make order start,request:{JsonConvert.SerializeObject(order)}");
            order.AppId = _config.AppId;
            order.MchId = _config.MchID;
            if (string.IsNullOrEmpty(order.NotifyUrl))
                order.NotifyUrl = _config.NotifyUrl;

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
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(MakeOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var obj = new JObject();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                var xmlNode = xmlDoc.FirstChild;
                var nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    var xe = (XmlElement)xn;
                    obj.Add(xe.Name, xe.InnerText);
                }
                _logger.LogInformation($"make order  successful,response:{result},status:{httpResponseMessage.StatusCode}");
                return JsonConvert.DeserializeObject<PaymentResult>(JsonConvert.SerializeObject(obj));
            }
            _logger.LogError($"make order failed,response:{result},status:{httpResponseMessage.StatusCode}");
            return null;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult> QueryOrderByOurOrderNoAsync(string orderNo)
        {
            _logger.LogInformation($"get order by OrderNo start,orderNo:{orderNo}");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", _config.AppId);
            dic.Add("mch_id", _config.MchID);
            dic.Add("out_trade_no", orderNo);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(GetOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var obj = new JObject();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                var xmlNode = xmlDoc.FirstChild;
                var nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    var xe = (XmlElement)xn;
                    obj.Add(xe.Name, xe.InnerText);
                }
                _logger.LogInformation($"get order by OrderNo successful,response:{result}");
                return JsonConvert.DeserializeObject<QueryOrderResult>(JsonConvert.SerializeObject(obj));
            }
            _logger.LogError($"get order by OrderNo failed,response:{result},status:{httpResponseMessage.StatusCode}");
            return null;
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="transactionId">微信订单号</param>
        /// <returns></returns>
        public async Task<QueryOrderResult> QueryOrderByWxIDAsync(string transactionId)
        {
            _logger.LogInformation($"get order by WxId start,transactionId:{transactionId}");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", _config.AppId);
            dic.Add("mch_id", _config.MchID);
            dic.Add("transaction_id", transactionId);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(GetOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {

                var obj = new JObject();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                var xmlNode = xmlDoc.FirstChild;
                var nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    var xe = (XmlElement)xn;
                    obj.Add(xe.Name, xe.InnerText);
                }
                _logger.LogInformation($"get order by WxId successful,response:{result}");
                return JsonConvert.DeserializeObject<QueryOrderResult>(JsonConvert.SerializeObject(obj));
            }
            _logger.LogError($"get order by WxId failed,response:{result},status:{httpResponseMessage.StatusCode}");
            return null;
        }

        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="orderNo">订单号</param>
        /// <param name="outRefundNo">退款单号</param>
        /// <param name="totalFee">总金额</param>
        /// <param name="refundFee">退款金额</param>
        /// <returns></returns>
        public async Task<RefundResult> RefundAsync(string orderNo, string outRefundNo, int totalFee, int refundFee)
        {
            _logger.LogInformation($"refund order start,orderNo:{orderNo},outRefundNo:{outRefundNo},totalFee：{totalFee},refundFee:{refundFee}");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("appid", _config.AppId);
            dic.Add("mch_id", _config.MchID);
            dic.Add("nonce_str", Guid.NewGuid().ToString("N"));
            dic.Add("sign_type", "MD5");
            dic.Add("out_trade_no", orderNo);
            dic.Add("out_refund_no", outRefundNo);
            dic.Add("total_fee", totalFee.ToString());
            dic.Add("refund_fee", refundFee.ToString());
            var sign = GetSign(dic);
            dic.Add("sign", sign);
            var xml = JsonConvert.DeserializeXmlNode(JsonConvert.SerializeObject(dic, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }), "xml").InnerXml;
            var httpClient = _httpClientFactory.CreateClient(Constant.Payment);
            var httpResponseMessage = await httpClient.PostAsync(RefundOrderUrl, new StringContent(xml, Encoding.UTF8, "application/xml"));
            var result = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var obj = new JObject();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                var xmlNode = xmlDoc.FirstChild;
                var nodes = xmlNode.ChildNodes;
                foreach (XmlNode xn in nodes)
                {
                    var xe = (XmlElement)xn;
                    obj.Add(xe.Name, xe.InnerText);
                }
                _logger.LogInformation($"refund order successful,response:{result}");
                return JsonConvert.DeserializeObject<RefundResult>(JsonConvert.SerializeObject(obj));
            }
            _logger.LogError($"refund order failed,response:{result},status:{httpResponseMessage.StatusCode}");
            return null;
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
            return await Task.FromResult(JsonConvert.DeserializeObject<NotifyResult>(JsonConvert.SerializeObject(obj)));

        }

        public string GetSign(Dictionary<string, string> dic)
        {
            dic = dic.OrderBy(q => q.Key).ToDictionary(q => q.Key, q => q.Value);
            var paramStr = GetParamStr(dic);
            var signTemp = $"{paramStr}&key={_config.Key}";
            return SHAmd5Encrypt(signTemp).ToUpper();
            string GetParamStr(Dictionary<string, string> param)
            {
                string p = "";
                foreach (var item in param)
                    p += $"{item.Key}={item.Value}&";
                return p.TrimEnd('&');
            }
        }

        private string SHAmd5Encrypt(string normalTxt)
        {
            var bytes = Encoding.Default.GetBytes(normalTxt);//求Byte[]数组  
            var Md5 = new MD5CryptoServiceProvider();
            var encryptbytes = Md5.ComputeHash(bytes);//求哈希值  
            string Base64To16(byte[] buffer)
            {
                string md_str = string.Empty;
                for (int i = 0; i < buffer.Length; i++)
                {
                    md_str += buffer[i].ToString("x2");
                }
                return md_str;
            }
            return Base64To16(encryptbytes);//将Byte[]数组转为净荷明文(其实就是字符串)  
        }

    }
}
