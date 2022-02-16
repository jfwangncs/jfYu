using Autofac;

namespace jfYu.Core.RabbitMQ
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMQService(this ContainerBuilder services)
        {
            services.Register(q => new RabbitMQService()).As<IRabbitMQService>();
        }


        /// <summary>
        /// IOC注入
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMQService(this ContainerBuilder services, string config)
        {
            services.Register(q => new RabbitMQService(config)).As<IRabbitMQService>();
        }
    }
}
