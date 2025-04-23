using Newtonsoft.Json;

namespace jfYu.Core.Wechat.Model.Payment
{
    public class UnifiedOrder
    {
        /// <summary>
        /// 微信分配的小程序ID
        /// </summary>
        [JsonProperty(PropertyName = "appid")]
        public string AppId { get; set; } = null!;

        /// <summary>
        /// 微信支付分配的商户号
        /// </summary>
        [JsonProperty(PropertyName = "mch_id")]
        public string MchId { get; set; } = null!;

        /// <summary>
        /// 自定义参数，可以为终端设备号(门店号或收银设备ID)，PC网页或公众号内支付可以传"WEB"
        /// </summary>
        [JsonProperty(PropertyName = "device_info")]
        public string DeviceInfo { get; set; } = null!;

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
        /// 签名类型，默认为MD5，支持HMAC-SHA256和MD5。
        /// </summary>
        [JsonProperty(PropertyName = "sign_type")]
        public string SignType { get; set; } = "MD5";

        /// <summary>
        /// 商品简单描述，该字段请按照规范传递
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public string Body { get; set; } = null!;

        /// <summary>
        /// 商品详细描述，对于使用单品优惠的商户，该字段必须按照规范上传
        /// </summary>
        [JsonProperty(PropertyName = "detail")]
        public string Detail { get; set; } = null!;

        /// <summary>
        /// 附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用。
        /// </summary>
        [JsonProperty(PropertyName = "attach")]
        public string Attach { get; set; } = null!;

        /// <summary>
        /// 商户系统内部订单号，要求32个字符内，只能是数字、大小写字母_-|*且在同一个商户号下唯一
        /// </summary>
        [JsonProperty(PropertyName = "out_trade_no")]
        public string OutTradeNo { get; set; } = null!;

        /// <summary>
        /// 符合ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        [JsonProperty(PropertyName = "fee_type")]
        public string FeeType { get; set; } = "CNY";

        /// <summary>
        /// 订单总金额，单位为分
        /// </summary>
        [JsonProperty(PropertyName = "total_fee")]
        public int TotalFee { get; set; }

        /// <summary>
        /// 终端IP
        /// </summary>
        [JsonProperty(PropertyName = "spbill_create_ip")]
        public string SpbillCreateIp { get; set; } = null!;

        /// <summary>
        /// 订单生成时间，格式为yyyyMMddHHmmss
        /// </summary>
        [JsonProperty(PropertyName = "time_start")]
        public string TimeStart { get; set; } = null!;

        /// <summary>
        /// 订单失效时间 1、最短失效时间间隔需大于1分钟。2、标准北京时间，时区为东八区，请务必保证服务器时间与当前北京时间一致。
        /// </summary>
        [JsonProperty(PropertyName = "time_expire")]
        public string TimeExpire { get; set; } = null!;

        /// <summary>
        /// 订单优惠标记，使用代金券或立减优惠功能时需要的参数
        /// </summary>
        [JsonProperty(PropertyName = "goods_tag")]
        public string GoodsTag { get; set; } = null!;

        /// <summary>
        /// 异步接收微信支付结果通知的回调地址，通知url必须为外网可访问的url，不能携带参数。公网域名必须为https，如果是走专线接入，使用专线NAT IP或者私有回调域名可使用http
        /// </summary>
        [JsonProperty(PropertyName = "notify_url")]
        public string NotifyUrl { get; set; } = null!;

        /// <summary>
        /// JSAPI--JSAPI支付（或小程序支付）、NATIVE--Native支付、APP--app支付，MWEB--H5支付
        /// </summary>
        [JsonProperty(PropertyName = "trade_type")]
        public string TradeType { get; set; } = null!;

        /// <summary>
        /// trade_type=NATIVE时，此参数必传。此参数为二维码中包含的商品ID，商户自行定义
        /// </summary>
        [JsonProperty(PropertyName = "product_id")]
        public string ProductId { get; set; } = null!;

        /// <summary>
        /// 上传此参数no_credit--可限制用户不能使用信用卡支付
        /// </summary>
        [JsonProperty(PropertyName = "limit_pay")]
        public string LimitPay { get; set; } = null!;

        /// <summary>
        /// trade_type=JSAPI，此参数必传，用户在商户appid下的唯一标识
        /// </summary>
        [JsonProperty(PropertyName = "openid")]
        public string Openid { get; set; } = null!;

        /// <summary>
        ///电子发票入口开放标识  Y，传入Y时，支付成功消息和支付详情页将出现开票入口。需要在微信支付商户平台或微信公众平台开通电子发票功能，传此字段才可生效
        /// </summary>
        [JsonProperty(PropertyName = "receipt")]
        public string Receipt { get; set; } = null!;

        /// <summary>
        /// Y-是，需要分账        N-否，不分账        字母要求大写，不传默认不分账
        /// </summary>
        [JsonProperty(PropertyName = "profit_sharing")]
        public string ProfitSharing { get; set; } = "N";

        /// <summary>
        /// 该字段常用于线下活动时的场景信息上报，支持上报实际门店信息，商户也可以按需求自己上报相关信息。该字段为JSON对象数据，对象格式为{"store_info":{"id": "门店ID","name": "名称","area_code": "编码","address": "地址" }}
        /// </summary>
        [JsonProperty(PropertyName = "scene_info")]
        public string SceneInfo { get; set; } = null!;
    }
}