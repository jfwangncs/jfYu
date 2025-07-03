using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JfYu.RabbitMQ
{
    /// <summary>
    /// RabbitMQService interface.
    /// </summary>
    public interface IRabbitMQService
    {
        /// <summary>
        /// Rabbit MQ connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Binds an exchange to another exchange with a routing key.
        /// </summary>
        /// <param name="destination">The destination exchange.</param>
        /// <param name="source">The source exchange.</param>
        /// <param name="exchangeType">The type of the exchange.<see cref="ExchangeType"/></param>
        /// <param name="routingKey">The routing key (optional).</param>
        /// <param name="headers">The headers (optional) if exchangeType="header" must be mandatory.</param>
        /// <returns>True if the binding is successful, otherwise false.</returns>
        bool ExchangeBind(string destination, string source, string exchangeType, string routingKey = "", Dictionary<string, object>? headers = null);

        /// <summary>
        /// Binds a queue to an exchange with a routing key.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="exchangeName">The name of the exchange.</param>
        /// <param name="exchangeType">The type of the exchange.<see cref="ExchangeType"/></param>
        /// <param name="routingKey">The routing key (optional).</param>
        /// <param name="headers">The headers (optional) if exchangeType="header" must be mandatory.</param>
        /// <returns>True if the binding is successful, otherwise false.</returns>
        bool QueueBind(string queueName, string exchangeName, string exchangeType, string routingKey = "", Dictionary<string, object>? headers = null);

        /// <summary>
        /// Receives messages from a queue with a specified prefetch count (synchronous).
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="func">The function to process the message.</param>
        /// <param name="prefetchCount">The number of messages to prefetch (default is 1).</param>
        IModel Receive(string queueName, Func<string, bool> func, ushort prefetchCount = 1);

        /// <summary>
        /// Receives messages from a queue asynchronously with a specified prefetch count.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="func">The async function to process the message.</param>
        /// <param name="prefetchCount">The number of messages to prefetch (default is 1).</param>
        IModel Receive(string queueName, Func<string, Task<bool>> func, ushort prefetchCount = 1);

        /// <summary>
        /// Receives messages from a queue with a specified prefetch count (synchronous).
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="func">The function to process the message.</param>
        /// <param name="prefetchCount">The number of messages to prefetch (default is 1).</param>
        IModel Receive<T>(string queueName, Func<T?, bool> func, ushort prefetchCount = 1);

        /// <summary>
        /// Receives messages from a queue asynchronously with a specified prefetch count.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="func">The async function to process the message.</param>
        /// <param name="prefetchCount">The number of messages to prefetch (default is 1).</param>
        IModel Receive<T>(string queueName, Func<T?, Task<bool>> func, ushort prefetchCount = 1);

        /// <summary>
        /// Sends a message to an exchange with a specific routing key.
        /// </summary>
        /// <typeparam name="T">The type of the message.</typeparam>
        /// <param name="exchangeName">The name of the exchange.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="routingKey">The routing key (optional).</param>
        /// <param name="headers">The headers (optional) if exchangeType="header" must be mandatory.</param>
        /// <returns>True if the message is sent successfully, otherwise false.</returns>
        bool Send<T>(string exchangeName, T msg, string routingKey = "", Dictionary<string, object>? headers = null);

        /// <summary>
        /// Sends a message to an exchange with a specific routing key.
        /// </summary>
        /// <param name="exchangeName">The name of the exchange.</param>
        /// <param name="msg">The message to send.</param>
        /// <param name="routingKey">The routing key (optional).</param>
        /// <param name="headers">The headers (optional) if exchangeType="header" must be mandatory.</param>
        /// <returns>True if the message is sent successfully, otherwise false.</returns>
        bool Send(string exchangeName, string msg, string routingKey = "", Dictionary<string, object>? headers = null);
    }
}