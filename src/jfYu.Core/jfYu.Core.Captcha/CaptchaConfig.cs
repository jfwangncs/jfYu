using System.Drawing;

namespace jfYu.Core.Captcha
{
    /// <summary>
    /// 验证码配置
    /// </summary>
    public class CaptchaConfig
    {
        /// <summary>
        /// 验证码种子库
        /// 例如：
        /// 2346789
        /// ABCDEFGHJKLMNPRTUVWXYZ
        /// 2346789ABCDEFGHJKLMNPRTUVWXYZ
        /// </summary>
        public string Characters { get; set; } = "2346789abcdefghjmnpqrtuxyzABCDEFGHJMNPQRTUXYZ";
        /// <summary>
        /// 验证码长度
        /// </summary>
        public int Length { get; set; } = 4;
        /// <summary>
        /// 验证码图片宽度
        /// </summary>
        public int Width { get; set; } = 120;
        /// <summary>
        /// 验证码图片高度
        /// </summary>
        public int Height { get; set; } = 32;
        /// <summary>
        /// 验证码背景颜色
        /// </summary>
        public Color BgColor { get; set; } = Color.FromArgb(236, 242, 244);
        /// <summary>
        /// 验证码颜色
        /// </summary>
        public Color[] FontColors { get; set; } 
    }
}
