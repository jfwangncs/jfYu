using Autofac;
using Microsoft.Extensions.Options;
using System;

namespace jfYu.Core.Common.Configurations.ConfigurableOptions
{
    public static class ConfigurableOptionsServiceCollectionExtensions
    {
        /// <summary>
        /// 添加选项配置
        /// </summary>
        /// <typeparam name="TOptions">选项类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static ContainerBuilder AddConfigurableOptions<TOptions>(this ContainerBuilder services)
            where TOptions : class, IConfigurableOptions
        {
            var optionsType = typeof(TOptions);

            // 获取键名
            var jsonKey = GetOptionsJsonKey(optionsType);

            // 配置选项（含验证信息）
            var configurationRoot = AppConfig.Configuration;
            var optionsConfiguration = configurationRoot.GetSection(jsonKey);


            services.Register(ctx => new ConfigurationChangeTokenSource<TOptions>(optionsType.Name, optionsConfiguration))
                .As<IOptionsChangeTokenSource<TOptions>>()
                .SingleInstance();

            services.Register(ctx => new NamedConfigureFromConfigurationOptions<TOptions>(optionsType.Name, optionsConfiguration, _ => { }))
                .As<IConfigureOptions<TOptions>>()
                .SingleInstance();

            return services;
        }

        /// <summary>
        /// 获取选项键
        /// </summary>
        /// <param name="optionsSettings">选项配置特性</param>
        /// <param name="optionsType">选项类型</param>
        /// <returns></returns>
        private static string GetOptionsJsonKey(Type optionsType)
        {
            // 默认后缀
            var defaultStuffx = nameof(Options);
            return optionsType.Name.EndsWith(defaultStuffx) ? optionsType.Name.Replace(defaultStuffx, "") : optionsType.Name;
        }
    }
}
