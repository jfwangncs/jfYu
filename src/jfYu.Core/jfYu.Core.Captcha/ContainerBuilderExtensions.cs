using Autofac;
using jfYu.Core.Configuration;
using Microsoft.Extensions.Options;

namespace jfYu.Core.Captcha
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCaptcha(this ContainerBuilder services)
        {             
            services.Register(q => new Captcha()).As<ICaptcha>().SingleInstance();
        }
        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCaptchaAsProperties(this ContainerBuilder services)
        {
            services.Register(q => new Captcha()).As<ICaptcha>().SingleInstance().PropertiesAutowired();
        }

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCaptcha(this ContainerBuilder services, CaptchaConfig captchaConfig)
        {
            services.Register(q => new Captcha(captchaConfig)).As<ICaptcha>().SingleInstance();
        }

        /// <summary>
        /// 验证码服务注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddCaptchaAsProperties(this ContainerBuilder services, CaptchaConfig captchaConfig)
        {
            services.Register(q => new Captcha(captchaConfig)).As<ICaptcha>().SingleInstance().PropertiesAutowired();
        }
    }
}
