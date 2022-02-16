using Autofac;
using NLog;
using NLog.Targets;

namespace jfYu.Core.Common.NLog
{

    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="nlogConfigFile">nlog配置文件地址</param>
        public static void AddNLog(this ContainerBuilder services, string nlogConfigFile = "nLog.config")
        {
            LogManager.LoadConfiguration(nlogConfigFile);

            services.Register(q => LogManager.GetLogger(q.GetType().FullName)).As<ILogger>().PropertiesAutowired();
        }
        /// <summary>
        /// ioc注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="nlogConfigFile">nlog配置文件地址</param>
        /// <param name="nlogDbString">数据库连接字符串</param>
        public static void AddNLog(this ContainerBuilder services, string nlogConfigFile = "nLog.config", string nlogDbString = "")
        {            
            LogManager.LoadConfiguration(nlogConfigFile);
            LogManager.Configuration.FindTargetByName<DatabaseTarget>("db").ConnectionString = nlogDbString;
            services.Register(q => LogManager.GetLogger(q.GetType().FullName)).As<ILogger>().PropertiesAutowired();
        }
    }

}
