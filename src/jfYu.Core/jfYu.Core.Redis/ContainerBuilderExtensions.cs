using Autofac;

namespace jfYu.Core.Redis
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddRedisService(this ContainerBuilder services)
        {
            services.Register(q => new RedisService()).As<RedisService>();
        }
    }
}
