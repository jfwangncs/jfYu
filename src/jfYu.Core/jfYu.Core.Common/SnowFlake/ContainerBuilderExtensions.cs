using Autofac;

namespace jfYu.Core.Common.SnowFlake
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// 雪花ID单例注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddSnowFlake(this ContainerBuilder services, int WorkerId = 1, int DataCenterId = 1)
        {
            services.Register(q => new SnowFlake(WorkerId, DataCenterId)).As<ISnowFlake>().SingleInstance();
        }
        /// <summary>
        /// 雪花ID单例注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddSnowFlakeAsProperties(this ContainerBuilder services, int WorkerId = 1, int DataCenterId = 1)
        {
            services.Register(q => new SnowFlake(WorkerId, DataCenterId)).As<ISnowFlake>().SingleInstance().PropertiesAutowired();
        }
    }
}
