using NLog;
using System;

namespace jfYu.Core.RabbitMQ
{
    public interface IRabbitMQService
    {
        /// <summary>
        /// 日志
        /// </summary>
        ILogger Logger { get; set; }


        /// <summary>
        /// MQ连接
        /// </summary>
        RabbitMQEndPoint RabbitMQConf { get; }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="queName">队列名</param>
        /// <param name="received">处理方法</param>
        void Receive(string queName, Action<string> received);

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="queName">队列名</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="exchangeType">模式类型fanout,direct,topic,headers </param>
        /// <param name="routingKey">通配符</param>
        /// <param name="received">处理方法</param>
        void Receive(string queName, string exchangeName, string exchangeType, Action<string> received, string routingKey = "");

        /// <summary>
        /// 发布订阅模式,路由模式,通配符模式发送消息
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="exchangeType">模式类型fanout,direct,topic,headers </param>
        /// <param name="key">通配符</param>
        /// <param name="msg">发送的消息</param>
        bool Send<T>(string exchangeName, string exchangeType, T msg, string key = "") where T : class;


        /// <summary>
        /// 一般模式，work模式
        /// </summary>       
        /// <param name="queName">队列名称</param>
        /// <param name="msg">发送的消息</param>
        bool Send<T>(string queName, T msg) where T : class;
    }
}
