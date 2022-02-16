using jfYu.Core.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace UnitTest4._7.Common
{


    [TestClass]
    public class TestCommon
    {
        [TestMethod]
        public void TestJson()
        {
            var builder = new ConfigurationBuilder().AddConfigurationFiles();
            Assert.AreEqual("Redis", AppConfig.Configuration.GetSection("Cache:Type").Value);
            Assert.AreEqual("2", AppConfig.Configuration.GetSection("Captcha:Length").Value);
            Assert.AreEqual("Redis", AppConfig.Configuration.GetSection("Cache:Type").Value);
        }

    }




}
