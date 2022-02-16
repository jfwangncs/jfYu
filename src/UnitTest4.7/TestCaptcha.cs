using Autofac;
using jfYu.Core.Captcha;
using jfYu.Core.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.IO;

namespace UnitTest4._7.Captcha
{


    [TestClass]
    public class TestCaptcha
    {
        [TestMethod]
        public void TestCaptchaDefault()
        {
            var con = new ContainerBuilder();

            con.AddCaptcha();
            var icon = con.Build();
            var Captcha = icon.Resolve<ICaptcha>();
            Assert.IsNotNull(Captcha);
            var result = Captcha.GetCaptcha();
            var filepath = SaveAsFile(result);
            Assert.IsTrue(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.AreEqual(120, img.Width);
            Assert.AreEqual(32, img.Height);
            Assert.AreEqual(4, result.CaptchaCode.Length);
            Assert.AreEqual(8, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
        }
        private string SaveAsFile(CaptchaResult result)
        {
            using (Stream s = new MemoryStream(result.CaptchaByteData))
            {
                byte[] srcBuf = new Byte[s.Length];
                s.Read(srcBuf, 0, srcBuf.Length);
                s.Seek(0, SeekOrigin.Begin);
                using (FileStream fs = new FileStream($"d:/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                }
                return $"d:/{result.CaptchaCode}.png";

            }
        }
    }


    [TestClass]
    public class TestCaptcha1
    {
        [TestMethod]
        public void TestCaptchaConfigJosn()
        {
            var con = new ContainerBuilder();
            var builder = new ConfigurationBuilder().AddConfigurationFile("Captcha.json", optional: true, reloadOnChange: true);
            con.AddCaptcha();
            var icon = con.Build();
            var Captcha = icon.Resolve<ICaptcha>();
            Assert.IsNotNull(Captcha);
            var result = Captcha.GetCaptcha();
            var filepath = SaveAsFile(result);
            Assert.IsTrue(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.AreEqual(241, img.Width);
            Assert.AreEqual(101, img.Height);
            Assert.AreEqual(2, result.CaptchaCode.Length);
            Assert.AreEqual(2, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
        }

        [TestMethod]
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
            Assert.IsNotNull(Captcha);
            var result = Captcha.GetCaptcha();
            var filepath = SaveAsFile(result);
            Assert.IsTrue(File.Exists(filepath));
            var img = Image.FromFile(filepath);
            Assert.AreEqual(360, img.Width);
            Assert.AreEqual(120, img.Height);
            Assert.AreEqual(6, result.CaptchaCode.Length);
            Assert.AreEqual(8, Captcha.CaptchaConfig.FontColors.Length);
            img.Dispose();
            File.Delete(filepath);
        }

        [TestMethod]
        public void TestCaptchaIOC()
        {
            var con = new ContainerBuilder();
            con.AddCaptchaAsProperties();
            con.RegisterType<SampleClassWithConstructorDependency>().AsSelf().PropertiesAutowired();
            var icon = con.Build();
            var axxx = icon.Resolve<SampleClassWithConstructorDependency>();
            var filepath = axxx.SampleMessage();
            Assert.IsTrue(File.Exists(filepath));
            File.Delete(filepath);
            Assert.IsNotNull(axxx.GetCaptcha());
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
                    using (FileStream fs = new FileStream($"d:/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(srcBuf, 0, srcBuf.Length);
                        fs.Close();
                    }
                    return $"d:/{result.CaptchaCode}.png";

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
                using (FileStream fs = new FileStream($"d:/{result.CaptchaCode}.png", FileMode.Create, FileAccess.Write))
                {
                    fs.Write(srcBuf, 0, srcBuf.Length);
                    fs.Close();
                }
                return $"d:/{result.CaptchaCode}.png";

            }
        }

    }
}
