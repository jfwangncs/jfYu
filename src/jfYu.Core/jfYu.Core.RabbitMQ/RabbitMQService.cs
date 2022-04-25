using jfYu.Core.Configuration;
using jfYu.Core.Common.Utilities;
using Microsoft.Extensions.Configuration;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;


namespace jfYu.Core.RabbitMQ
{

    public class RabbitMQService : IRabbitMQService
    {

        /// <summary>
        /// 日志
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// MQ连接
        /// </summary>
        private IConnection Con { get; set; }

        /// <summary>
        /// MQ配置
        /// </summary>
        public RabbitMQEndPoint RabbitMQConf { get; } = new RabbitMQEndPoint();

        public RabbitMQService()
        {
            try
            {
                RabbitMQConf = AppConfig.Configuration.GetSection("RabbitMQ")?.Get<RabbitMQEndPoint>();
                var factory = new ConnectionFactory//创建连接工厂对象
                {
                    HostName = RabbitMQConf.HostName,
                    Port = RabbitMQConf.Port,
                    UserName = RabbitMQConf.UserName,
                    Password = RabbitMQConf.Password,
                    VirtualHost=RabbitMQConf.VirtualHost,
                    RequestedHeartbeat = TimeSpan.FromSeconds(RabbitMQConf.HeartBeat),
                    AutomaticRecoveryEnabled = true //自动重连

                };
                Con = factory.CreateConnection();//创建连接对象
            }
            catch (Exception ex)
            {
                Logger?.Error($"无法连接RabbitMQ:{ex.Message}-{ex.StackTrace}-{Serializer.Serialize(RabbitMQConf)}");
                throw new Exception($"无法连接RabbitMQ:{ex.Message}-{ex.StackTrace}");
            }

        }
        public RabbitMQService(string connstr)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(connstr),
                    AutomaticRecoveryEnabled = true
                };
                Con = factory.CreateConnection();//创建连接对象
            }
            catch (Exception ex)
            {
                Logger?.Error(ex, $"无法连接RabbitMQ:{connstr}");
                throw new Exception($"无法连接RabbitMQ:{ex.Message}-{ex.StackTrace}");
            }

        }

        #region 发送

        /// <summary>
        /// 一般模式，work模式
        /// </summary>       
        /// <param name="queName">队列名称</param>
        /// <param name="msg">发送的消息</param>
        public bool Send<T>(string queName, T msg) where T : class
        {
            //发送重连
            if (!Con.IsOpen)
            {
                while (true)
                {
                    if (Con.IsOpen)
                        break;
                    Task.Delay(Con.Heartbeat).Wait();
                }
            }
            using var channel = Con.CreateModel();
            channel.ConfirmSelect(); //确认发送成功否
            //声明一个队列
            channel.QueueDeclare(queName, true, false, false, null);
            var basicProperties = channel.CreateBasicProperties();
            //1：非持久化 2：可持久化
            basicProperties.DeliveryMode = 2;
            var payload = Encoding.UTF8.GetBytes(Serializer.Serialize(msg));
            channel.BasicPublish("", queName, basicProperties, payload);
            return channel.WaitForConfirms();
        }

        /// <summary>
        /// 发布订阅模式,路由模式,通配符模式发送消息
        /// </summary>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="exchangeType">模式类型fanout,direct,topic,headers  </param>
        /// <param name="key">通配符</param>
        /// <param name="msg">发送的消息</param>
        public bool Send<T>(string exchangeName, string exchangeType, T msg, string key = "") where T : class
        {
            key = exchangeType == "fanout" ? "" : key;
            //发送重连
            if (!Con.IsOpen)
            {
                while (true)
                {
                    if (Con.IsOpen)
                        break;
                    Task.Delay(Con.Heartbeat).Wait();
                }
            }
            using var channel = Con.CreateModel();
            channel.ConfirmSelect(); //确认发送成功否
            //发送方确认机制
            channel.ConfirmSelect();
            //声明exchange
            channel.ExchangeDeclare(exchangeName, exchangeType, true);
            var basicProperties = channel.CreateBasicProperties();
            //1：非持久化 2：可持久化
            basicProperties.DeliveryMode = 2;
            var payload = Encoding.UTF8.GetBytes(Serializer.Serialize(msg));
            channel.BasicPublish(exchangeName, key, basicProperties, payload);
            return channel.WaitForConfirms();
        }

        #endregion

        #region 接收


        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="queName">队列名</param>
        /// <param name="received">处理方法</param>
        public void Receive(string queName, Action<string> received)
        {
            var channel = Con.CreateModel();
            // 声明队列
            channel.QueueDeclare(queName, true, false, false, null);
            //每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息
            channel.BasicQos(0, 1, false);
            //事件基本消费者
            var consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    received(message);
                    //确认该消息已被消费
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                };

            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queName, false, consumer);
        }

        /// <summary>
        /// 接受消息
        /// </summary>
        /// <param name="queName">队列名</param>
        /// <param name="exchangeName">交换机名称</param>
        /// <param name="routingKey">通配符</param>
        /// <param name="received">处理方法</param>
        public void Receive(string queName, string exchangeName, string exchangeType, Action<string> received, string routingKey = "")
        {

            var channel = Con.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(exchangeName, exchangeType, true);
            // 声明队列
            channel.QueueDeclare(queName, true, false, false, null);
            //绑定到交换机
            channel.QueueBind(queName, exchangeName, routingKey);
            //每次只能向消费者发送一条信息,再消费者未确认之前,不再向他发送信息
            channel.BasicQos(0, 1, false);
            //事件基本消费者
            var consumer = new EventingBasicConsumer(channel);

            //接收到消息事件
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    received(message);
                    //确认该消息已被消费
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                }

            };
            //启动消费者 设置为手动应答消息
            channel.BasicConsume(queName, false, consumer);
        }

        #endregion

    }
}
