
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;

namespace jfYu.Core.RabbitMQ
{

    /// <summary>
    ///  Adds RabbitMQ services extensions
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds RabbitMQ services to the IServiceCollection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the services to.</param>
        /// <param name="configureConnectionFactory">An action to configure the ConnectionFactory and MessageRetryPolicy.</param>
        /// <returns>The IServiceCollection with the RabbitMQ services added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when configureConnectionFactory is null.</exception>
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
