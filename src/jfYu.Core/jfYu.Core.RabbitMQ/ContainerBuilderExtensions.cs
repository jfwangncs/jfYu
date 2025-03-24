
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace jfYu.Core.RabbitMQ
{
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        public static IServiceCollection AddRabbitMQService(this IServiceCollection services, Action<ConnectionFactory, MessageRetryPolicy> configureConnectionFactory)
        {
            if (configureConnectionFactory == null)
                throw new ArgumentNullException(nameof(configureConnectionFactory), "The configureConnectionFactory action cannot be null.");
             
            var option = new ConnectionFactory();
            var messageRetryPolicy = new MessageRetryPolicy();
            configureConnectionFactory(option, messageRetryPolicy);

            var connection = option.CreateConnection();
            services.AddSingleton(connection);
            services.AddSingleton(messageRetryPolicy);
            services.AddTransient(provider =>
            {
                return connection.CreateModel();
            });
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            return services;
        }
    }
}
