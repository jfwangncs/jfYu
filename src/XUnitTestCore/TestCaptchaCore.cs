using Autofac;
using jfYu.Core.Captcha;
using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.IO;
using Xunit;


namespace xUnitTestCore.CaptchaCore
{

    public class TestCaptchaCore1
    {
        [Fact]
        public void TestCaptchaDefault()
        {

            var con = new ContainerBuilder();

            con.AddCaptcha();
            var icon = con.Build();
            var Captcha = icon.Resolve<ICaptcha>();
            Assert.NotNull(Captcha);
            var result = Captcha.GetCaptcha();
            if (!Directory.Exists("captchatest"))
            {
                Directory.CreateDirectory("captchatest");
            }
            var filepath = SaveAsFile(result);
            Assert.True(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.Equal(120, img.Width);
            Assert.Equal(32, img.Height);
            Assert.Equal(4, result.CaptchaCode.Length);
            Assert.Equal(8, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
            if (Directory.Exists("captchatest"))
            {
                Directory.Delete("captchatest");
            }
        }

        private string SaveAsFile(CaptchaResult result)
        {
            using (Stream s = new MemoryStream(result.CaptchaByteData))
            {
                byte[] srcBuf = new Byte[s.Length];
                s.Read(srcBuf, 0, srcBuf.Length);
                s.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream($"captchatest/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                }
                return $"captchatest/{result.CaptchaCode}.png";

            }
        }
    }
    public class TestCaptchaCore2
    {

        [Fact]
        public void TestCaptchaConfigJosn()
        {
            var con = new ContainerBuilder();
            var builder = new ConfigurationBuilder().AddConfigurationFile("Captcha.json", optional: true, reloadOnChange: true);
            con.AddCaptcha();
            var icon = con.Build();
            var Captcha = icon.Resolve<ICaptcha>();
            Assert.NotNull(Captcha);
            var result = Captcha.GetCaptcha();
            if (!Directory.Exists("captchatest"))
            {
                Directory.CreateDirectory("captchatest");
            }
            var filepath = SaveAsFile(result);
            Assert.True(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.Equal(241, img.Width);
            Assert.Equal(101, img.Height);
            Assert.Equal(2, result.CaptchaCode.Length);
            Assert.Equal(2, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
            if (Directory.Exists("captchatest"))
            {
                Directory.Delete("captchatest");
            }
        }

        [Fact]
        public void TestCaptchaConfig()
        {
            var con = new ContainerBuilder();
            CaptchaConfig captchaConfig = new CaptchaConfig();
            captchaConfig.Length = 6;
            captchaConfig.Width = 360;
            captchaConfig.Height = 120;
            con.AddCaptcha(captchaConfig);
            var icon = con.Build();
            var Captcha = icon.Resolve<ICaptcha>();
            Assert.NotNull(Captcha);
            var result = Captcha.GetCaptcha();
            if (!Directory.Exists("captchatest"))
            {
                Directory.CreateDirectory("captchatest");
            }
            var filepath = SaveAsFile(result);
            Assert.True(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.Equal(360, img.Width);
            Assert.Equal(120, img.Height);
            Assert.Equal(6, result.CaptchaCode.Length);
            Assert.Equal(8, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
            if (Directory.Exists("captchatest"))
            {
                Directory.Delete("captchatest");
            }
        }

        [Fact]
        public void TestCaptchaIOC()
        {
            var con = new ContainerBuilder();
            con.AddCaptchaAsProperties();
            con.RegisterType<SampleClassWithConstructorDependency>().AsSelf().PropertiesAutowired();
            var icon = con.Build();
            var axxx = icon.Resolve<SampleClassWithConstructorDependency>();
            if (!Directory.Exists("captchatest"))
            {
                Directory.CreateDirectory("captchatest");
            }
            var filepath = axxx.SampleMessage();
            Assert.True(File.Exists(filepath));
            File.Delete(filepath);
            Assert.NotNull(axxx.GetCaptcha());
            if (Directory.Exists("captchatest"))
            {
                Directory.Delete("captchatest");
            };
        }
        public class SampleClassWithConstructorDependency
        {
            public ICaptcha Captcha { get; set; }

            public SampleClassWithConstructorDependency()
            {

            }

            public string SampleMessage()
            {
                var result = Captcha.GetCaptcha();
                var filepath = SaveAsFile(result);
                return filepath;
            }

            public ICaptcha GetCaptcha()
            {
                return Captcha;
            }
            private string SaveAsFile(CaptchaResult result)
            {
                using (Stream s = new MemoryStream(result.CaptchaByteData))
                {
                    byte[] srcBuf = new Byte[s.Length];
                    s.Read(srcBuf, 0, srcBuf.Length);
                    s.Seek(0, SeekOrigin.Begin);
                    using (FileStream fs = new FileStream($"captchatest/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(srcBuf, 0, srcBuf.Length);
                        fs.Close();
                    }
                    return $"captchatest/{result.CaptchaCode}.png";

                }
            }
        }


        private string SaveAsFile(CaptchaResult result)
        {
            using (Stream s = new MemoryStream(result.CaptchaByteData))
            {
                byte[] srcBuf = new Byte[s.Length];
                s.Read(srcBuf, 0, srcBuf.Length);
                s.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream($"captchatest/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                }
                return $"captchatest/{result.CaptchaCode}.png";

            }
        }
    }
}
