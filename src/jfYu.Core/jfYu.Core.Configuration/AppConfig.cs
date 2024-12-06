using Microsoft.Extensions.Configuration;

namespace jfYu.Core.Configuration
{
    public static class AppConfig
    {
        /// <summary>
        /// 全局配置选项
        /// </summary>
        public static IConfiguration Configuration { get; set; }


        /// <summary>
        /// 全局配置构建器
        /// </summary>
        internal static IConfigurationBuilder ConfigurationBuilder { get; set; }

    }
}
