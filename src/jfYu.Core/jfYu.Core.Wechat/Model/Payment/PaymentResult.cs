using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model.Payment
{
    public class PaymentBaseResult
    {
        [JsonProperty(PropertyName = "return_code")]
        public string ReturnCode { get; set; } = null!;

        [JsonProperty(PropertyName = "return_msg")]
        public string ReturnMsg { get; set; } = null!;

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        [JsonProperty(PropertyName = "mch_id")]
        public string MchId { get; set; } = null!;

        /// <summary>
        /// 微信分配的小程序ID
        /// </summary>
        [JsonProperty(PropertyName = "appid")]
        public string AppId { get; set; } = null!;


        /// <summary>
        /// 随机字符串，长度要求在32位以内
        /// </summary>
        [JsonProperty(PropertyName = "nonce_str")]
        public string NonceStr { get; set; } = null!;

        /// <summary>
        /// 通过签名算法计算得出的签名值
        /// </summary>
        [JsonProperty(PropertyName = "sign")]
        public string Sign { get; set; } = null!;

        /// <summary>
        /// SUCCESS/FAIL
        /// </summary>
        [JsonProperty(PropertyName = "result_code")]
        public string ResultCode { get; set; } = null!;

        /// <summary>
        /// 详细参见下文错误列表
        /// </summary>
        [JsonProperty(PropertyName = "err_code")]
        public string ErrCode { get; set; } = null!;

        /// <summary>
        /// 错误信息描述
        /// </summary>
        [JsonProperty(PropertyName = "err_code_des")]
        public string ErrCodeDes { get; set; } = null!;
    }
    public class PaymentResult : PaymentBaseResult
    {
        /// <summary>
        ///交易类型，取值为：JSAPI，NATIVE，APP等
        /// </summary>
        [JsonProperty(PropertyName = "trade_type")]
        public string TradeType { get; set; } = null!;

        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        [JsonProperty(PropertyName = "prepay_id")]
        public string PrepayId { get; set; } = null!;

        /// <summary>
        /// 此url用于生成支付二维码，然后提供给用户进行扫码支付。 
        /// 注意：code_url的值并非固定，使用时按照URL格式转成二维码即可。时效性为2小时
        /// </summary>
        [JsonProperty(PropertyName = "code_url")]
        public string CodeUrl { get; set; } = null!;

    } 
    public class RefundResult: PaymentBaseResult
    { 
    
    }
}
