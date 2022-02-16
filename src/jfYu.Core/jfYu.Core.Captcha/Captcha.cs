using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace jfYu.Core.Captcha
{
    public class Captcha : ICaptcha
    {
        public CaptchaConfig CaptchaConfig { get; }
        public Captcha()
        {
            //读取配置文件
            try
            {
                CaptchaConfig = AppConfig.Configuration?.GetSection("Captcha")?.Get<CaptchaConfig>() ?? new CaptchaConfig()
                {
                    FontColors = new Color[] {
                        Color.FromArgb(44, 62, 80),Color.FromArgb(192, 57, 43),Color.FromArgb(22, 160, 133),
                        Color.FromArgb(192, 57, 42), Color.FromArgb(142, 68, 173),
                        Color.FromArgb(48, 63, 159), Color.FromArgb(245, 124, 0), Color.FromArgb(121, 85, 72)
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("读取配置文件出错", ex);
            }
        }

        public Captcha(CaptchaConfig captchaConfig)
        {
            CaptchaConfig = captchaConfig ?? new CaptchaConfig();

            captchaConfig.FontColors = captchaConfig.FontColors ?? new Color[] {
                        Color.FromArgb(44, 62, 80),Color.FromArgb(192, 57, 43),Color.FromArgb(22, 160, 133),
                        Color.FromArgb(192, 57, 42), Color.FromArgb(142, 68, 173),
                        Color.FromArgb(48, 63, 159), Color.FromArgb(245, 124, 0), Color.FromArgb(121, 85, 72)
                    };
        }


        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns>验证码相关信息</returns>
        public CaptchaResult GetCaptcha()
        {
            //生成验证码
            StringBuilder sb = new StringBuilder();
            Random rand = new Random();
            int maxRand = CaptchaConfig.Characters.Length - 1;
            for (int i = 0; i < CaptchaConfig.Length; i++)
            {
                int index = rand.Next(maxRand);
                sb.Append(CaptchaConfig.Characters[index]);
            }
            string captchaCode = sb.ToString();
            //生成图片
            MemoryStream ms = new MemoryStream();
            using (Bitmap baseMap = new Bitmap(CaptchaConfig.Width, CaptchaConfig.Height))
            using (Graphics graph = Graphics.FromImage(baseMap))
            {

                graph.Clear(CaptchaConfig.BgColor);
                DrawCaptchaCode();
                DrawDisorderLine();
                DrawDisorderPoint();
                AdjustRippleEffect();
                baseMap.Save(ms, ImageFormat.Png);

                static int GetFontSize(int imageWidth, int captchCodeCount)
                {
                    var averageSize = imageWidth / captchCodeCount;
                    return Convert.ToInt32(averageSize);
                }

                Color GetRandomDeepColor()
                {
                    Random rand = new Random();
                    int maxRand = CaptchaConfig.FontColors.Length - 1;
                    int index = rand.Next(maxRand);
                    return CaptchaConfig.FontColors[index];
                }


                void DrawCaptchaCode()
                {
                    SolidBrush fontBrush = new SolidBrush(Color.Black);

                    int fontSize = GetFontSize(CaptchaConfig.Width, captchaCode.Length);
                    Font font = new Font(FontFamily.GenericSerif, fontSize, GraphicsUnit.Pixel);
                    for (int i = 0; i < captchaCode.Length; i++)
                    {
                        fontBrush.Color = GetRandomDeepColor();
                        int shiftPx = fontSize / 6;
                        float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                        int maxY = CaptchaConfig.Height - fontSize;
                        if (maxY < 0) maxY = 0;
                        float y = rand.Next(0, maxY);
                        graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
                    }
                }

                void DrawDisorderLine()
                {
                    Pen linePen = new Pen(new SolidBrush(Color.Black), 2);
                    int maxLines = CaptchaConfig.Width > 150 ? 15 : 10;
                    for (int i = 0; i < rand.Next(5, maxLines); i++)
                    {
                        linePen.Color = GetRandomDeepColor();

                        Point startPoint = new Point(rand.Next(0, CaptchaConfig.Width), rand.Next(0, CaptchaConfig.Height));
                        Point endPoint = new Point(rand.Next(0, CaptchaConfig.Width), rand.Next(0, CaptchaConfig.Height));
                        graph.DrawLine(linePen, startPoint, endPoint);

                    }
                }


                void DrawDisorderPoint()
                {
                    for (int i = 0; i < rand.Next(100, 150); i++)
                    {
                        baseMap.SetPixel(rand.Next(0, CaptchaConfig.Width), rand.Next(0, CaptchaConfig.Height), Color.FromArgb(rand.Next()));
                    }
                }

                void AdjustRippleEffect()
                {
                    short nWave = 6;
                    int nWidth = baseMap.Width;
                    int nHeight = baseMap.Height;
                    Point[,] pt = new Point[nWidth, nHeight];
                    for (int x = 0; x < nWidth; ++x)
                    {
                        for (int y = 0; y < nHeight; ++y)
                        {
                            var xo = nWave * Math.Sin(2.0 * 3.1415 * y / 128.0);
                            var yo = nWave * Math.Cos(2.0 * 3.1415 * x / 128.0);

                            var newX = x + xo;
                            var newY = y + yo;

                            if (newX > 0 && newX < nWidth)
                            {
                                pt[x, y].X = (int)newX;
                            }
                            else
                            {
                                pt[x, y].X = 0;
                            }
                            if (newY > 0 && newY < nHeight)
                            {
                                pt[x, y].Y = (int)newY;
                            }
                            else
                            {
                                pt[x, y].Y = 0;
                            }
                        }
                    }
                    Bitmap bSrc = (Bitmap)baseMap.Clone();
                    BitmapData bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                    int scanline = bitmapData.Stride;
                    IntPtr scan0 = bitmapData.Scan0;
                    IntPtr srcScan0 = bmSrc.Scan0;
                    //unsafe
                    //{
                    //    byte* p = (byte*)(void*)scan0;
                    //    byte* pSrc = (byte*)(void*)srcScan0;

                    //    int nOffset = bitmapData.Stride - baseMap.Width * 3;

                    //    for (int y = 0; y < nHeight; ++y)
                    //    {
                    //        for (int x = 0; x < nWidth; ++x)
                    //        {
                    //            var xOffset = pt[x, y].X;
                    //            var yOffset = pt[x, y].Y;

                    //            if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                    //            {
                    //                if (pSrc != null)
                    //                {
                    //                    p[0] = pSrc[yOffset * scanline + xOffset * 3];
                    //                    p[1] = pSrc[yOffset * scanline + xOffset * 3 + 1];
                    //                    p[2] = pSrc[yOffset * scanline + xOffset * 3 + 2];
                    //                }
                    //            }

                    //            p += 3;
                    //        }
                    //        p += nOffset;
                    //    }
                    //}
                    baseMap.UnlockBits(bitmapData);
                    bSrc.UnlockBits(bmSrc);
                    bSrc.Dispose();
                }
            }
            return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
        }

        /// <summary>
        /// [颜色：16进制转成RGB]
        /// </summary>
        /// <param name="strColor">设置16进制颜色 [返回RGB]</param>
        /// <returns></returns>
        Color FromHx16(string strHxColor)
        {
            try
            {
                if (strHxColor.Length == 0)
                {//如果为空
                    return Color.FromArgb(0, 0, 0);//设为黑色
                }
                else
                {//转换颜色
                    return Color.FromArgb(
                        Int32.Parse(strHxColor.Substring(1, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                        Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                        Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                }
            }
            catch
            {//设为黑色
                return Color.FromArgb(0, 0, 0);
            }
        }
    }
}
