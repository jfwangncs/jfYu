using RabbitMQ.Client;
using System;
using System.Threading.Tasks;

namespace jfYu.Core.RabbitMQ
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// Rabbit clinet
        /// </summary>
        ConnectionFactory Factory { get; }

        /// <summary>
        /// simple mode
        /// </summary>
        /// <param name="queName">queue name</param>
        /// <param name="msg"></param>
        /// <returns>successful/failed</returns>
        bool Send(string queueName, object msg);

        /// <summary>
        /// Publish/Subscribe,Routing,Topics mode
        /// </summary>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="exchangeType">exchange type</param>
        /// <param name="msg">msg</param>
        /// <param name="routingKey">key</param>
        /// <returns>successful/failed</returns>
        bool Send(string exchangeName, ExchangeType exchangeType, object msg, string routingKey = "");

        /// <summary>
        /// receive
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="func">function</param>
        void Receive(string queueName, Action<string> func);

        /// <summary>
        /// receive
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="exchangeType">exchange type</param>
        /// <param name="func">function</param>
        /// <param name="routingKey">routing key</param>
        void Receive(string queueName, string exchangeName, string exchangeType, Action<string> func, string routingKey = "");

        /// <summary>
        /// receive
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="exchangeType">exchange type</param>
        /// <param name="func">function</param>
        /// <param name="routingKey">routing key</param>
        void Receive(string queueName, string exchangeName, string exchangeType, Func<string, Task> func, string routingKey = "");


        /// <summary>
        /// receive
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="func">function</param>
        void Receive(string queueName, Func<string, Task> func);

        /// <summary>
        /// QueueBind
        /// </summary>
        /// <param name="queueName">queue name</param>
        /// <param name="exchangeName">exchange name</param>
        /// <param name="exchangeType">exchange type</param>
        /// <param name="routingKey">routing key</param>
        /// <returns></returns>
        bool QueueBind(string queueName, string exchangeName, ExchangeType exchangeType, string routingKey = "");

        /// <summary>
        /// ExchangeBind
        /// </summary>
        /// <param name="destination">destination exchange name</param>
        /// <param name="source">source exchange nam</param>
        /// <param name="exchangeType">exchange type</param>
        /// <param name="routingKey">routing key</param>
        /// <returns></returns>
        bool ExchangeBind(string destination, string source, ExchangeType exchangeType, string routingKey = "");
    }
}
