using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace jfYu.Core.Configuration
{
    /// <summary>
    /// 配置文件扩展类
    /// </summary>
    public static class ConfigurationExtensions
    {

        /// <summary>
        /// 获取配置文件内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="optional">文件是可选的</param>
        /// <param name="reloadOnChange">更改后重载文件</param>
        public static IConfigurationBuilder AddConfigurationFile(this IConfigurationBuilder builder, string path, bool optional = true, bool reloadOnChange = true)
        {
            return AddConfigurationFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        /// <summary>
        /// 获取配置文件内容
        /// </summary>
        /// <param name="provider">读取文件提供程序</param>
        /// <param name="path">文件路径</param>
        /// <param name="optional">文件是可选的</param>
        /// <param name="reloadOnChange">更改后重载文件</param>
        public static IConfigurationBuilder AddConfigurationFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (File.Exists(path))
            {
                if (provider == null)
                {
                    builder.AddJsonFile(path, optional, reloadOnChange);
                }
                else
                {
                    builder.AddJsonFile(provider, path, optional, reloadOnChange);
                }
                AppConfig.Configuration = builder.Build();
            }
            else
            {
                throw new FileNotFoundException($"找不到{path}文件.");
            }
            return builder;
        }

        /// <summary>
        /// 添加配置文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        public static IConfigurationBuilder AddConfigurationFiles(this IConfigurationBuilder config, IHostEnvironment env)
        {
            var appsettingsConfiguration = config.Build();
            // 读取忽略的配置文件
            var ignoreConfigurationFiles = appsettingsConfiguration
                    .GetSection("IgnoreConfigurationFiles")
                    .Get<string[]>()
                ?? Array.Empty<string>();

            // 加载配置
            AutoAddJsonFiles(config, env, ignoreConfigurationFiles);

            // 存储配置
            AppConfig.ConfigurationBuilder = config;
            AppConfig.Configuration = config.Build();
            return config;
        }

        /// <summary>
        /// 添加配置文件,appsetting.json和appsetting.{environment}.json中具有相同Key，则使用appsetting.json中数据
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        public static IConfigurationBuilder AddConfigurationFiles(this IConfigurationBuilder config)
        {
            var appsettingsConfiguration = config.Build();
            // 读取忽略的配置文件
            var ignoreConfigurationFiles = appsettingsConfiguration
                    .GetSection("IgnoreConfigurationFiles")
                    .Get<string[]>()
                ?? Array.Empty<string>();

            // 加载配置
            AutoAddJsonFiles(config, ignoreConfigurationFiles);

            // 存储配置
            AppConfig.ConfigurationBuilder = config;
            AppConfig.Configuration = config.Build();
            return config;
        }

        /// <summary>
        /// 加载目录下配置文件
        /// </summary>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <param name="ignoreConfigurationFiles"></param>
        private static void AutoAddJsonFiles(IConfigurationBuilder config, IHostEnvironment env, string[] ignoreConfigurationFiles)
        {
            // 获取程序目录下的所有配置文件
            var jsonFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.json", SearchOption.TopDirectoryOnly)
                .Union(
                    Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.TopDirectoryOnly)
                )
                .Where(u => CheckIncludeDefaultSettings(Path.GetFileName(u)) && !ignoreConfigurationFiles.Contains(Path.GetFileName(u)) && !runtimeJsonSuffixs.Any(j => u.EndsWith(j)));

            if (!jsonFiles.Any()) return;

            // 获取环境变量名
            var envName = env.EnvironmentName;
            var envFiles = new List<string>();

            // 自动加载配置文件
            foreach (var jsonFile in jsonFiles)
            {
                // 处理带环境的配置文件
                if (Path.GetFileNameWithoutExtension(jsonFile).EndsWith($".{envName}"))
                {
                    envFiles.Add(jsonFile);
                    continue;
                }

                config.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);
            }

            // 配置带环境的配置文件
            envFiles.ForEach(u => config.AddJsonFile(u, optional: true, reloadOnChange: true));
        }

        private static void AutoAddJsonFiles(IConfigurationBuilder config, string[] ignoreConfigurationFiles)
        {
            // 获取程序目录下的所有配置文件
            var jsonFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.json", SearchOption.TopDirectoryOnly)
                .Union(
                    Directory.GetFiles(Directory.GetCurrentDirectory(), "*.json", SearchOption.TopDirectoryOnly)
                )
                .Where(u => CheckIncludeDefaultSettings(Path.GetFileName(u)) && !ignoreConfigurationFiles.Contains(Path.GetFileName(u)) && !runtimeJsonSuffixs.Any(j => u.EndsWith(j)))
                .OrderBy(u => u);

            if (!jsonFiles.Any()) return;

            // 自动加载配置文件
            foreach (var jsonFile in jsonFiles)
            {
                config.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);
            }
        }

        ///<summary>
        /// 排除特定配置文件正则表达式
        /// </summary>
        private const string excludeJsonPattern = @"^{0}(\.\w+)?\.((json)|(xml))$";

        /// <summary>
        /// 排序的配置文件前缀
        /// </summary>
        private static readonly string[] excludeJsonPrefixs = new[] { "bundleconfig", "compilerconfig" };

        /// <summary>
        /// 检查是否受排除的配置文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool CheckIncludeDefaultSettings(string fileName)
        {
            var isTrue = true;
            foreach (var prefix in excludeJsonPrefixs)
            {
                var isMatch = Regex.IsMatch(fileName, string.Format(excludeJsonPattern, prefix));
                if (isMatch)
                {
                    isTrue = false;
                    break;
                }
            }
            return isTrue;
        }

        /// <summary>
        /// 排除运行时 Json 后缀
        /// </summary>
        private static readonly string[] runtimeJsonSuffixs = new[]
        {
            "deps.json",
            "runtimeconfig.dev.json",
            "runtimeconfig.prod.json",
            "runtimeconfig.json"
        };

    }

}
