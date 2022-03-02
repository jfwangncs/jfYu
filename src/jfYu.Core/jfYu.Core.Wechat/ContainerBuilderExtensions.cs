using Autofac;

namespace jfYu.Core.Wechat
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgram(this ContainerBuilder services)
        {
            services.Register(q => new MiniProgram()).SingleInstance();
        }
        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgramAsProperties(this ContainerBuilder services)
        {
            services.Register(q => new MiniProgram()).SingleInstance().PropertiesAutowired();
        }

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgram(this ContainerBuilder services, WechatConfig captchaConfig)
        {
            services.Register(q => new MiniProgram(captchaConfig)).SingleInstance();
        }

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgramAsProperties(this ContainerBuilder services, WechatConfig captchaConfig)
        {
            services.Register(q => new MiniProgram(captchaConfig)).SingleInstance().PropertiesAutowired();
        }
    }
}
