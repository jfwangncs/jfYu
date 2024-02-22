
using Microsoft.Extensions.DependencyInjection;
using System;

namespace jfYu.Core.RabbitMQ
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMQService(this IServiceCollection services, RabbitMQConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrEmpty(config.HostName))
                throw new ArgumentNullException(nameof(config.HostName));

            services.AddSingleton(config);
            services.AddScoped<IRabbitMQService, RabbitMQService>();
        }
    }
}
