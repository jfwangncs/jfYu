using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace jfYu.Core.Configuration
{
    public static class ConfigurableOptionsServiceCollectionExtensions
    {


        /// <summary>
        /// 添加选项配置
        /// </summary>
        /// <typeparam name="TOptions">选项类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddConfigurableOptions<TOptions>(this IServiceCollection services)
            where TOptions : class, IConfigurableOptions
        {
            var optionsType = typeof(TOptions);

            // 获取键名
            var jsonKey = GetOptionsJsonKey(optionsType);

            // 配置选项（含验证信息）
            var configurationRoot = AppConfig.Configuration;
            var optionsConfiguration = configurationRoot.GetSection(jsonKey);
            if(!optionsConfiguration.Exists())
            {
                throw new Exception($"找不到{jsonKey}配置。");    
            }
            // 配置选项监听
            if (typeof(IConfigurableOptionsListener<TOptions>).IsAssignableFrom(optionsType))
            {
                var onListenerMethod = optionsType.GetMethod(nameof(IConfigurableOptionsListener<TOptions>.OnListener));
                if (onListenerMethod != null)
                {
                    ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
                    {
                        var options = optionsConfiguration.Get<TOptions>();
                        onListenerMethod.Invoke(options, new object[] { options, optionsConfiguration });
                    });
                }
            }

            services.AddOptions<TOptions>()
                .Bind(optionsConfiguration, options =>
                {
                    options.BindNonPublicProperties = true; // 绑定私有变量
                })
                .ValidateDataAnnotations();

            // 配置复杂验证后后期配置
            var validateInterface = optionsType.GetInterfaces()
                .FirstOrDefault(u => u.IsGenericType && typeof(IConfigurableOptions).IsAssignableFrom(u.GetGenericTypeDefinition()));
            if (validateInterface != null)
            {
                var genericArguments = validateInterface.GenericTypeArguments;

                // 配置复杂验证
                if (genericArguments.Length > 1)
                {
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IValidateOptions<TOptions>), genericArguments.Last()));
                }

                // 配置后期配置
                var postConfigureMethod = optionsType.GetMethod(nameof(IConfigurableOptions<TOptions>.PostConfigure));
                if (postConfigureMethod != null)
                {
                    services.PostConfigure<TOptions>(options => postConfigureMethod.Invoke(options, new object[] { options, optionsConfiguration }));

                }
            }

            return services;
        }


        ///// <summary>
        ///// 添加选项配置
        ///// </summary>
        ///// <typeparam name="TOptions">选项类型</typeparam>
        ///// <param name="services">服务集合</param>
        ///// <returns>服务集合</returns>
        //public static ContainerBuilder AddConfigurableOptions<TOptions>(this ContainerBuilder services)
        //    where TOptions : class, IConfigurableOptions
        //{
        //    var optionsType = typeof(TOptions);

        //    // 获取键名
        //    var jsonKey = GetOptionsJsonKey(optionsType);

        //    // 配置选项（含验证信息）
        //    var configurationRoot = AppConfig.Configuration;
        //    var optionsConfiguration = configurationRoot.GetSection(jsonKey);
        //    // 配置选项监听
        //    if (typeof(IConfigurableOptionsListener<TOptions>).IsAssignableFrom(optionsType))
        //    {
        //        var onListenerMethod = optionsType.GetMethod(nameof(IConfigurableOptionsListener<TOptions>.OnListener));
        //        if (onListenerMethod != null)
        //        {
        //            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
        //            {
        //                var options = optionsConfiguration.Get<TOptions>();
        //                onListenerMethod.Invoke(options, new object[] { options, optionsConfiguration });
        //            });
        //        }
        //    }
        //    var options = optionsConfiguration.Get<TOptions>();
        //    if (options != null)
        //    {
        //        services.Register(ctx => options).As<TOptions>().SingleInstance();
        //    }
        //    else
        //        throw new Exception($"找不到{typeof(TOptions).Name}相关配置");
        //    return services;
        //}

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
