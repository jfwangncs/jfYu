using Autofac;

namespace jfYu.Core.Wechat
{
    public static class ContainerBuilderExtensions
    {

        #region 小程序
        /// <summary>
        /// 小程序
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgram(this ContainerBuilder services)
        {
            services.Register(q => new MiniProgram()).SingleInstance();
        }
        /// <summary>
        /// 小程序
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgramAsProperties(this ContainerBuilder services)
        {
            services.Register(q => new MiniProgram()).SingleInstance().PropertiesAutowired();
        }

        /// <summary>
        /// 小程序
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgram(this ContainerBuilder services, WechatConfig captchaConfig)
        {
            services.Register(q => new MiniProgram(captchaConfig)).SingleInstance();
        }

        /// <summary>
        /// 小程序
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgramAsProperties(this ContainerBuilder services, WechatConfig captchaConfig)
        {
            services.Register(q => new MiniProgram(captchaConfig)).SingleInstance().PropertiesAutowired();
        }

        #endregion

        #region 支付
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="services"></param>
        public static void AddWechatPayment(this ContainerBuilder services)
        {
            services.Register(q => new WechatPayment()).SingleInstance();
        }
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="services"></param>
        public static void AddWechatPaymentAsProperties(this ContainerBuilder services)
        {
            services.Register(q => new WechatPayment()).SingleInstance().PropertiesAutowired();
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="services"></param>
        public static void AddWechatPayment(this ContainerBuilder services, PaymentConfig config)
        {
            services.Register(q => new WechatPayment(config)).SingleInstance();
        }

        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="services"></param>
        public static void AddWechatPaymentAsProperties(this ContainerBuilder services, PaymentConfig config)
        {
            services.Register(q => new WechatPayment(config)).SingleInstance().PropertiesAutowired();
        } 
        #endregion
    }
}
