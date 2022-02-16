namespace jfYu.Core.Captcha
{
    public interface ICaptcha
    {
        CaptchaConfig CaptchaConfig { get; }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns>验证码相关信息</returns>
        CaptchaResult GetCaptcha();
    }
}