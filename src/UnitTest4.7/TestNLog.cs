using Autofac;
using jfYu.Core.Common.Configurations;
using jfYu.Core.Common.NLog;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;

namespace UnitTest4._7.NLog
{
    [TestClass]
    public class TestNLog
    {



        [TestMethod]
        public void Test1()
        {
            var config = new ConfigurationBuilder().AddConfigurationFile("SqlServer.json", true, true);
            config.Build();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddNLog("nLog.config");

            var _container = containerBuilder.Build();
            var logger1 = _container.Resolve<ILogger>();
            Assert.IsNotNull(logger1);


            var containerBuilder1 = new ContainerBuilder();
            containerBuilder1.AddNLog("nLog.config", AppConfig.Configuration.GetSection("ConnectionStrings:MasterConnectionString").Value);

            var _container1 = containerBuilder1.Build();
            var logger2 = _container1.Resolve<ILogger>();
            Assert.IsNotNull(logger2);
            logger2.Trace("xx3");


        }
    }

}
