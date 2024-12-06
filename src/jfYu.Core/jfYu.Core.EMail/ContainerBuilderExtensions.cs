using Autofac;

namespace jfYu.Core.EMail
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// ioc注册
        /// </summary>
        /// <param name="services"></param>
        public static void AddEmail(this ContainerBuilder services)
        {
            services.Register(q=>new Email()).As<IEmail>().SingleInstance();
        }
        /// <summary>
        /// ioc注册
        /// </summary>
        /// <param name="services"></param>
        /// <param name="emailConfiguration">邮件配置信息</param>
        public static void AddEmail(this ContainerBuilder services, EmailConfiguration emailconfiguration)
        {
            services.Register(c => new Email(emailconfiguration)).As<IEmail>().SingleInstance();
        }
    }
}
