using Autofac;
using AutoMapper;
using NLog;
using NLog.Targets;
using System;

namespace jfYu.Core.Common.AutoMapper
{

    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="nlogConfigFile">nlog配置文件地址</param>
        public static void AddAutoMapper(this ContainerBuilder services, Action<IMapperConfigurationExpression> func)
        {
            //此处进行automapper配置
            services.Register<IConfigurationProvider>(q => new MapperConfiguration(c=>func(c))).PropertiesAutowired();
            services.Register<IMapper>(ctx => new Mapper(ctx.Resolve<IConfigurationProvider>(), ctx.Resolve)).InstancePerDependency().PropertiesAutowired();

        }      
    }

}
