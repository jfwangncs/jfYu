using Autofac;

namespace jfYu.Core.Common.SnowFlake
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 雪花ID单例注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddSnowFlake(this ContainerBuilder services)
        {
            services.Register(q => new SnowFlake(1, 1)).As<ISnowFlake>().SingleInstance();
        }
        /// <summary>
        /// 雪花ID单例注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddSnowFlakeAsProperties(this ContainerBuilder services)
        {
            services.Register(q => new SnowFlake(1, 1)).As<ISnowFlake>().SingleInstance().PropertiesAutowired();
        }
    }
}
