using jfYu.Core.Common.Configurations;
using jfYu.Core.Common.Utilities;
using Microsoft.Extensions.Configuration;
using Xunit;


namespace xUnitTestCore.Common
{

    public class TestConfigCore
    {
        [Fact]
        public void TestJson()
        {
            new ConfigurationBuilder().AddConfigurationFiles();
            Assert.Equal("2", AppConfig.Configuration["Captcha:Length"]);
            Assert.Equal("Redis", AppConfig.Configuration["Cache:Type"]);
        }
    }

    public class TestEncryptCore
    {
        [Fact]
        public void AesEncryptTest()
        {
            string s = "王进锋";
            string s1 = "jfwang123";
            var spwd = s.AesEncrypt();
            var s1pwd = s1.AesEncrypt();
            Assert.Equal(spwd.AesDecrypt(), s);
            Assert.Equal(s1pwd.AesDecrypt(), s1);
        }
        [Fact]
        public void DesEncryptTest()
        {
            string s = "王进锋";
            string s1 = "jfwang123";
            var spwd = s.DesEncrypt();
            var s1pwd = s1.DesEncrypt();
            Assert.Equal(spwd.DesDecrypt(), s);
            Assert.Equal(s1pwd.DesDecrypt(), s1);
        }

        [Fact]
        public void RasEncryptTest()
        {
            RSAEncrypt Rsa = new RSAEncrypt();
            Rsa.GenerateKeys("d://pems");
            string s = "王进锋";
            string s1 = "jfwang123";
            var spwd = Rsa.Encrypt(s, "d://pems/RSA.Pub");
            var s1pwd = Rsa.Encrypt(s1, "d://pems/RSA.Pub");
            Assert.Equal(Rsa.Decrypt(spwd, "d://pems/RSA.Private"), s);
            Assert.Equal(Rsa.Decrypt(s1pwd, "d://pems/RSA.Private"), s1);
        }
    }

}
