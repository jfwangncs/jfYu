using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model.Payment
{
    public class PaymentBaseResult
    {
        [JsonProperty(PropertyName = "return_code")]
        public string ReturnCode { get; set; }

        [JsonProperty(PropertyName = "return_msg")]
        public string ReturnMsg { get; set; }

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        [JsonProperty(PropertyName = "mch_id")]
        public string MchId { get; set; }

        /// <summary>
        /// 微信分配的小程序ID
        /// </summary>
        [JsonProperty(PropertyName = "appid")]
        public string AppId { get; set; }


        /// <summary>
        /// 随机字符串，长度要求在32位以内
        /// </summary>
        [JsonProperty(PropertyName = "nonce_str")]
        public string NonceStr { get; set; }

        /// <summary>
        /// 通过签名算法计算得出的签名值
        /// </summary>
        [JsonProperty(PropertyName = "sign")]
        public string Sign { get; set; }

        /// <summary>
        /// SUCCESS/FAIL
        /// </summary>
        [JsonProperty(PropertyName = "result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 详细参见下文错误列表
        /// </summary>
        [JsonProperty(PropertyName = "err_code")]
        public string ErrCode { get; set; }

        /// <summary>
        /// 错误信息描述
        /// </summary>
        [JsonProperty(PropertyName = "err_code_des")]
        public string ErrCodeDes { get; set; }
    }
    public class PaymentResult : PaymentBaseResult
    {
        /// <summary>
        ///交易类型，取值为：JSAPI，NATIVE，APP等
        /// </summary>
        [JsonProperty(PropertyName = "trade_type")]
        public string TradeType { get; set; }

        /// <summary>
        /// 微信生成的预支付会话标识，用于后续接口调用中使用，该值有效期为2小时
        /// </summary>
        [JsonProperty(PropertyName = "prepay_id")]
        public string PrepayId { get; set; }

        /// <summary>
        /// 此url用于生成支付二维码，然后提供给用户进行扫码支付。 
        /// 注意：code_url的值并非固定，使用时按照URL格式转成二维码即可。时效性为2小时
        /// </summary>
        [JsonProperty(PropertyName = "code_url")]
        public string CodeUrl { get; set; }

    }

    public class QueryOrderResult: PaymentBaseResult
    {
        /// <summary>
        /// 设备号
        /// </summary>
        [JsonProperty(PropertyName = "device_info")]
        public string DeviceInfo { get; set; }

        /// <summary>
        /// 用户标识
        /// </summary>
        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; }
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        [JsonProperty(PropertyName = "is_subscribe")]
        public string IsSubscribe { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        [JsonProperty(PropertyName = "trade_type")]
        public string TradeType { get; set; }
        /// <summary>
        /// 交易状态
        /// </summary>
        [JsonProperty(PropertyName = "trade_state")]
        public string TradeState { get; set; }
        /// <summary>
        /// 付款银行
        /// </summary>
        [JsonProperty(PropertyName = "bank_type")]
        public string BankType { get; set; }

        /// <summary>
        /// 标价金额
        /// </summary>
        [JsonProperty(PropertyName = "total_fee")]
        public string TotalFee { get; set; }
        /// <summary>
        /// 应结订单金额
        /// </summary>
        [JsonProperty(PropertyName = "settlement_total_fee")]
        public string SettlementTotalFee { get; set; }
        /// <summary>
        /// 标价币种
        /// </summary>
        [JsonProperty(PropertyName = "fee_type")]
        public string FeeType { get; set; }
        /// <summary>
        /// 现金支付金额
        /// </summary>
        [JsonProperty(PropertyName = "cash_fee")]
        public string CashFee { get; set; }
        /// <summary>
        /// 现金支付币种
        /// </summary>
        [JsonProperty(PropertyName = "cash_fee_type")]
        public string CashFeeType { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        [JsonProperty(PropertyName = "out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        [JsonProperty(PropertyName = "attach")]
        public string Attach { get; set; }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        [JsonProperty(PropertyName = "time_end")]
        public string TimeEnd { get; set; }
        /// <summary>
        /// 交易状态描述
        /// </summary>
        [JsonProperty(PropertyName = "trade_state_desc")]
        public string TradeStateDesc { get; set; }

    }
    public class RefundResult: PaymentBaseResult
    { 
    
    }
}
