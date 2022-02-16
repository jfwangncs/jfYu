using Autofac;
using jfYu.Core.Common.Configurations;
using jfYu.Core.EMail;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest4._7.Mail
{

    [TestClass]
    public class TestMail
    {     


        [TestMethod]
        public void SendEmail1()
        {

            var builder = new ConfigurationBuilder()
             .AddConfigurationFile("Email.json", optional: true, reloadOnChange: true);
            builder.Build();
            var cb = new ContainerBuilder();
            cb.AddEmail();
            IEmail mail = cb.Build().Resolve<IEmail>();
            Assert.IsNotNull(mail);
            mail.SendMail("475760135@qq.com", "单TO无CC", "测试邮件");
            mail.SendMail("475760135@qq.com", "617110271@qq.com", "单TO单CC", "测试邮件");
            mail.SendMail("475760135@qq.com", "617110271@qq.com;wjf@sunbirddata.com.cn", "单TO多CC", "测试邮件");
            mail.SendMail("475760135@qq.com;617110271@qq.com", "多TO无CC", "测试邮件");
            mail.SendMail("475760135@qq.com;617110271@qq.com", "617110271@qq.com", "多TO单CC", "测试邮件");
            mail.SendMail("475760135@qq.com;617110271@qq.com", "617110271@qq.com;wjf@sunbirddata.com.cn", "多TO多CC", "测试邮件");


        }

        [TestMethod]
        public void SendEmail2()
        {
            var builder = new ConfigurationBuilder()
               .AddConfigurationFile("Email.json", optional: true, reloadOnChange: true);
            builder.Build();
            var cb = new ContainerBuilder();
            cb.AddEmail();
            IEmail mail = cb.Build().Resolve<IEmail>();
            Assert.IsNotNull(mail);
            mail.SendMailAsync("475760135@qq.com", "单TO无CC", "测试邮件").Wait();
            mail.SendMailAsync("475760135@qq.com", "617110271@qq.com", "单TO单CC", "测试邮件").Wait();
            mail.SendMailAsync("475760135@qq.com", "617110271@qq.com;wjf@sunbirddata.com.cn", "单TO多CC", "测试邮件").Wait();
            mail.SendMailAsync("475760135@qq.com;617110271@qq.com", "多TO无CC", "测试邮件").Wait();
            mail.SendMailAsync("475760135@qq.com;617110271@qq.com", "617110271@qq.com", "多TO单CC", "测试邮件").Wait();
            mail.SendMailAsync("475760135@qq.com;617110271@qq.com", "617110271@qq.com;wjf@sunbirddata.com.cn", "多TO多CC", "测试邮件").Wait();
        }    
    }

}
