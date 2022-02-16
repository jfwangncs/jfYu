using Autofac;
using Autofac.Extensions.DependencyInjection;
using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;


namespace xUnitTestCore.ConfigCore
{

    public class TestConfigCore
    {

        [Fact]
        public void TestAddNotFoundFile()
        {
            Assert.Throws<FileNotFoundException>(() => new ConfigurationBuilder().AddConfigurationFile("x.json", true, true));
            Assert.Throws<FileNotFoundException>(() => new ConfigurationBuilder().AddConfigurationFile("d:/x.json", true, true));
        }
        [Fact]
        public void TestAddRelativeFile()
        {
            new ConfigurationBuilder().AddConfigurationFile("CacheRedis.json", true, true);
            Assert.Equal("5000", AppConfig.Configuration["Redis:Timeout"]);
            Assert.Equal("5000", AppConfig.Configuration.GetSection("Redis").GetSection("Timeout").Value);
            Assert.Equal("Redis", AppConfig.Configuration["Cache:Type"]);
        }

        [Fact]
        public void TestAddAbsoluteFile()
        {
            new ConfigurationBuilder().AddConfigurationFile($"{Directory.GetCurrentDirectory()}/CacheMemory.json", true, true);
            Assert.Equal("Memory", AppConfig.Configuration["Cache:Type"]);
        }
        [Fact]
        public void TestAddAllFile()
        {
            new ConfigurationBuilder().AddConfigurationFiles();
            Assert.Equal("2", AppConfig.Configuration["Captcha:Length"]);
            Assert.Equal("Redis", AppConfig.Configuration["Cache:Type"]);
        }

        [Fact]
        public void TestOptionsAllFile()
        {
            IServiceCollection services = new ServiceCollection();
            new ConfigurationBuilder().AddConfigurationFiles();
            services.AddConfigurableOptions<RedisOptions>();
            ContainerBuilder containerBuild = new ContainerBuilder();
            containerBuild.Populate(services);
            containerBuild.RegisterType<OptionServer>();
            var container = containerBuild.Build();
            var options = container.Resolve<OptionServer>();
            Assert.Equal(5000, options.GetOptions().Timeout);
            Assert.Equal("Password", options.GetOptions().Password);
            Assert.Equal("Host1", options.GetOptions().EndPoints[0].Host);
            Assert.Equal(1111, options.GetOptions().EndPoints[0].Port);
            Assert.Equal("Host2", options.GetOptions().EndPoints[1].Host);
            Assert.Equal(2222, options.GetOptions().EndPoints[1].Port);
        }

        [Fact]
        public void TestOptionsEnvFile()
        {
            var services = new ServiceCollection();
            new ConfigurationBuilder().AddConfigurationFiles(new MyHostEnvironment());
            services.AddConfigurableOptions<RedisOptions>();
            ContainerBuilder containerBuild = new ContainerBuilder();
            containerBuild.Populate(services);
            containerBuild.RegisterType<OptionServer>();
            var container = containerBuild.Build();
            var options = container.Resolve<OptionServer>();
            Assert.Equal(5001, options.GetOptions().Timeout);
            Assert.Equal("Password1", options.GetOptions().Password);
            Assert.Equal("Host11", options.GetOptions().EndPoints[0].Host);
            Assert.Equal(11111, options.GetOptions().EndPoints[0].Port);
            Assert.Equal("Host21", options.GetOptions().EndPoints[1].Host);
            Assert.Equal(22221, options.GetOptions().EndPoints[1].Port);
        }

        [Fact]
        public void TestOptionsSingleFile()
        {

            new ConfigurationBuilder().AddConfigurationFile("CacheRedis.json", true, true);
            var services = new ServiceCollection();
            services.AddConfigurableOptions<RedisOptions>();
            ContainerBuilder containerBuild = new ContainerBuilder();
            containerBuild.Populate(services);
            containerBuild.RegisterType<OptionServer>();
            var container = containerBuild.Build();
            var options = container.Resolve<OptionServer>();
            Assert.Equal(5000, options.GetOptions().Timeout);
            Assert.Equal("Password", options.GetOptions().Password);
            Assert.Equal("Host1", options.GetOptions().EndPoints[0].Host);
            Assert.Equal(1111, options.GetOptions().EndPoints[0].Port);
            Assert.Equal("Host2", options.GetOptions().EndPoints[1].Host);
            Assert.Equal(2222, options.GetOptions().EndPoints[1].Port);
        }

        [Fact]
        public void TestOptionsNotFoundFile()
        {
            var services = new ServiceCollection();
            new ConfigurationBuilder().AddConfigurationFile("CacheMemory.json", true, true);
            Assert.Throws<Exception>(() => services.AddConfigurableOptions<RedisOptions>());

        }

        [Fact]
        public void TestOptionsMonitor()
        {
            IServiceCollection services = new ServiceCollection();
            new ConfigurationBuilder().AddConfigurationFiles();
            services.AddConfigurableOptions<RedisOptions>();
            ContainerBuilder containerBuild = new ContainerBuilder();
            containerBuild.Populate(services);
            containerBuild.RegisterType<OptionMonitorServer>();
            var container = containerBuild.Build();
            var options = container.Resolve<OptionMonitorServer>();
            Assert.Equal(5000, options.GetOptions().Timeout);
            Assert.Equal("Password", options.GetOptions().Password);
            Assert.Equal("Host1", options.GetOptions().EndPoints[0].Host);
            Assert.Equal(1111, options.GetOptions().EndPoints[0].Port);
            Assert.Equal("Host2", options.GetOptions().EndPoints[1].Host);
            Assert.Equal(2222, options.GetOptions().EndPoints[1].Port);
        }


    }

    public class MyHostEnvironment : IHostEnvironment
    {

        public string EnvironmentName { get => Environments.Development; set => throw new System.NotImplementedException(); }
        public string ApplicationName { get => "ApplicationName"; set => throw new System.NotImplementedException(); }
        public string ContentRootPath { get => "ContentRootPath"; set => throw new System.NotImplementedException(); }
        public IFileProvider ContentRootFileProvider { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    }

    public class OptionServer
    {
        private RedisOptions RedisOptions;
        public OptionServer(IOptions<RedisOptions> config)
        {
            RedisOptions = config.Value;
        }

        public RedisOptions GetOptions()
        {
            return RedisOptions;
        }
    }

    public class OptionMonitorServer
    {
        private RedisOptions RedisOptions;
        public OptionMonitorServer(IOptionsMonitor<RedisOptions> config)
        {
            RedisOptions = config.CurrentValue;
            config.OnChange(q =>
            {
                RedisOptions = config.CurrentValue;
            });
        }

        public RedisOptions GetOptions()
        {
            return RedisOptions;
        }
    }

    public class OptionSnapshotrServer
    {
        private RedisOptions RedisOptions;
        public OptionSnapshotrServer(IOptionsSnapshot<RedisOptions> config)
        {
            RedisOptions = config.Value;
        }

        public RedisOptions GetOptions()
        {
            return RedisOptions;
        }
    }
    public class RedisOptions : IConfigurableOptions
    {

        public List<RedisEndPoint> EndPoints { get; set; }
        /// <summary>
        /// 密码
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// 数据库index
        /// </summary>

        public int DbIndex { get; set; } = 0;

        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>

        public int Timeout { get; set; } = 5000;

    }

    public class RedisEndPoint
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
