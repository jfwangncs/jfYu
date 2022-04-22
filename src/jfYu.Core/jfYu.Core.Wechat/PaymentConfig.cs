namespace jfYu.Core.Wechat
{
    public class PaymentConfig
    {
        /// <summary>
        /// appid
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 商户Id
        /// </summary>
        public string MchID { get; set; }

        /// <summary>
        /// key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 证书地址
        /// </summary>
        public string CertPath { get; set; }

        /// <summary>
        /// 证书密钥
        /// </summary>
        public string KeyPath { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        public string NotifyUrl { get; set; }
        

    }
}
