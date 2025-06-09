namespace jfYu.Core.Wechat.Config
{
    public class PaymentConfig
    {
        /// <summary>
        /// appid
        /// </summary>
        public string AppId { get; set; } = null!;

        /// <summary>
        /// 商户Id
        /// </summary>
        public string MchID { get; set; } = null!;

        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; } = null!;

        /// <summary>
        /// 证书地址
        /// </summary>
        public string CertPath { get; set; } = null!;

        /// <summary>
        /// 证书密钥
        /// </summary>
        public string KeyPath { get; set; } = null!;

        /// <summary>
        /// 通知地址
        /// </summary>
        public string NotifyUrl { get; set; } = null!;
    }
}