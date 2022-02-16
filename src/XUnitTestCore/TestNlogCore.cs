using Autofac;
using NLog;
using Xunit;
using jfYu.Core.Common.NLog;
using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;

namespace xUnitTestCore.NLog
{
    public class TestNlogCore
    {
        [Fact]
        public void CreateNlog()
        {
            var config = new ConfigurationBuilder().AddConfigurationFile("SqlServer.json", true, true);
            config.Build();
            var containerBuilder = new ContainerBuilder();
            containerBuilder.AddNLog("nLog.config");
            containerBuilder.RegisterType<SampleClassWithConstructorDependency>();
            containerBuilder.RegisterType<SampleClassWithPropertiesAutowired>().PropertiesAutowired();
            var _container = containerBuilder.Build();
            var CLogger = _container.Resolve<SampleClassWithConstructorDependency>();
            Assert.NotNull(CLogger.GetLogger());
            var PLogger = _container.Resolve<SampleClassWithPropertiesAutowired>();
            Assert.NotNull(PLogger._logger);
            CLogger.GetLogger().Trace("x1");
            PLogger._logger.Trace("x2");

            var containerBuilder1 = new ContainerBuilder();
            containerBuilder1.AddNLog("nLog.config", AppConfig.Configuration.GetSection("ConnectionStrings:MasterConnectionString").Value);
            containerBuilder1.RegisterType<SampleClassWithConstructorDependency>();
            containerBuilder1.RegisterType<SampleClassWithPropertiesAutowired>().PropertiesAutowired();
            var _container1 = containerBuilder1.Build();
            var CLogger1 = _container1.Resolve<SampleClassWithConstructorDependency>();
            Assert.NotNull(CLogger1.GetLogger());
            var PLogger1 = _container1.Resolve<SampleClassWithPropertiesAutowired>();
            Assert.NotNull(PLogger1._logger);
            CLogger1.GetLogger().Trace("x3");
            PLogger1._logger.Trace("x4");
            PLogger1._logger.AddLog(LogLevel.Info, "¥ÌŒÛ", "“Ï≥£");

        }


        public class SampleClassWithConstructorDependency
        {
            private readonly ILogger _logger;

            public SampleClassWithConstructorDependency(ILogger logger)
            {
                _logger = logger;
            }

            public void SampleMessage(string message)
            {
                _logger.Debug(message);
            }

            public ILogger GetLogger()
            {
                return _logger;
            }
        }

        public class SampleClassWithPropertiesAutowired
        {
            public ILogger _logger { get; set; }

            public void SampleMessage(string message)
            {
                _logger.Debug(message);
            }
        }
    }
}
