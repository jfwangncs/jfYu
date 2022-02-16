using System;

namespace jfYu.Core.Captcha
{
    public class CaptchaResult
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string CaptchaCode { get; set; }
        /// <summary>
        /// 验证码图片字节数据
        /// </summary>
        public byte[] CaptchaByteData { get; set; }
        /// <summary>
        /// 验证码图片Base64数据
        /// </summary>
        public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
