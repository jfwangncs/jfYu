using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model.Payment
{
    public class QueryOrderResult : PaymentBaseResult
    {
        /// <summary>
        /// 设备号
        /// </summary>
        [JsonProperty(PropertyName = "device_info")]
        public string DeviceInfo { get; set; } = null!;

        /// <summary>
        /// 用户标识
        /// </summary>
        [JsonProperty(PropertyName = "openid")]
        public string OpenId { get; set; } = null!;
        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        [JsonProperty(PropertyName = "is_subscribe")]
        public string IsSubscribe { get; set; } = null!;
        /// <summary>
        /// 交易类型
        /// </summary>
        [JsonProperty(PropertyName = "trade_type")]
        public string TradeType { get; set; } = null!;
        /// <summary>
        /// 交易状态
        /// </summary>
        [JsonProperty(PropertyName = "trade_state")]
        public string TradeState { get; set; } = null!;
        /// <summary>
        /// 付款银行
        /// </summary>
        [JsonProperty(PropertyName = "bank_type")]
        public string BankType { get; set; } = null!;

        /// <summary>
        /// 标价金额
        /// </summary>
        [JsonProperty(PropertyName = "total_fee")]
        public string TotalFee { get; set; } = null!;
        /// <summary>
        /// 应结订单金额
        /// </summary>
        [JsonProperty(PropertyName = "settlement_total_fee")]
        public string SettlementTotalFee { get; set; } = null!;
        /// <summary>
        /// 标价币种
        /// </summary>
        [JsonProperty(PropertyName = "fee_type")]
        public string FeeType { get; set; } = null!;
        /// <summary>
        /// 现金支付金额
        /// </summary>
        [JsonProperty(PropertyName = "cash_fee")]
        public string CashFee { get; set; } = null!;
        /// <summary>
        /// 现金支付币种
        /// </summary>
        [JsonProperty(PropertyName = "cash_fee_type")]
        public string CashFeeType { get; set; } = null!;

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        [JsonProperty(PropertyName = "transaction_id")]
        public string TransactionId { get; set; } = null!;
        /// <summary>
        /// 商户订单号
        /// </summary>
        [JsonProperty(PropertyName = "out_trade_no")]
        public string OutTradeNo { get; set; } = null!;

        /// <summary>
        /// 附加数据
        /// </summary>
        [JsonProperty(PropertyName = "attach")]
        public string Attach { get; set; } = null!;
        /// <summary>
        /// 支付完成时间
        /// </summary>
        [JsonProperty(PropertyName = "time_end")]
        public string TimeEnd { get; set; } = null!;
        /// <summary>
        /// 交易状态描述
        /// </summary>
        [JsonProperty(PropertyName = "trade_state_desc")]
        public string TradeStateDesc { get; set; } = null!;

    }

    public class NotifyResult : QueryOrderResult
    {

    }
}
