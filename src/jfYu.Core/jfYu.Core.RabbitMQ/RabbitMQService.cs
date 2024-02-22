using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;


namespace jfYu.Core.RabbitMQ
{

    public class RabbitMQService : IRabbitMQService
    {
        public ConnectionFactory Factory { get; } 

        public RabbitMQService(RabbitMQConfig config)
        {
            try
            {
                Factory = new ConnectionFactory
                {
                    HostName = config.HostName,
                    Port = config.Port,
                    UserName = config.UserName,
                    Password = config.Password,
                    VirtualHost = config.VirtualHost,
                    RequestedHeartbeat = TimeSpan.FromSeconds(config.HeartBeat),
                    AutomaticRecoveryEnabled = true, 

                };
            }
            catch (Exception)
            {
                throw;
            }

        }
        public bool QueueBind(string queueName, string exchangeName, ExchangeType exchangeType, string routingKey = "")
        {
            Factory.DispatchConsumersAsync = false;
            using var connection = Factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ConfirmSelect(); // confirm           
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.ExchangeDeclare(exchangeName, exchangeType.ToString().ToLower(), true);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            channel.QueueBind(queueName, exchangeName, routingKey);
            return channel.WaitForConfirms();
        }

        public bool ExchangeBind(string destination, string source, ExchangeType exchangeType, string routingKey = "")
        {
            Factory.DispatchConsumersAsync = false;
            using var connection = Factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ConfirmSelect(); // confirm
            channel.ExchangeDeclare(destination, exchangeType.ToString().ToLower(), true);
            channel.ExchangeDeclare(source, exchangeType.ToString().ToLower(), true);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            channel.ExchangeBind(destination, source, routingKey);
            return channel.WaitForConfirms();
        }


        public bool Send(string queueName, object msg)
        {
            Factory.DispatchConsumersAsync = false;
            using var connection = Factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ConfirmSelect(); // confirm           
            channel.QueueDeclare(queueName, true, false, false, null);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            channel.BasicPublish("", queueName, basicProperties, payload);
            return channel.WaitForConfirms();
        }

        public bool Send(string exchangeName, ExchangeType exchangeType, object msg, string routingKey = "")
        {
            Factory.DispatchConsumersAsync = false;
            routingKey = exchangeType == ExchangeType.Fanout ? "" : routingKey;
            using var connection = Factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.ConfirmSelect();
            channel.ExchangeDeclare(exchangeName, exchangeType.ToString().ToLower(), true);
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            channel.BasicPublish(exchangeName, routingKey, basicProperties, payload);
            return channel.WaitForConfirms();
        }


        public void Receive(string queueName, Action<string> func)
        {
            Factory.DispatchConsumersAsync = false;
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);
            //consume one by one
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    func(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                };

            };
            //manually confirm
            channel.BasicConsume(queueName, false, consumer);
        }

        public void ReceiveAsync(string queueName, Func<string, Task> func)
        {
            Factory.DispatchConsumersAsync = true;
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName, true, false, false, null);
            //consume one by one
            channel.BasicQos(0, 1, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await func(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                };

            };
            //manually confirm
            channel.BasicConsume(queueName, false, consumer);
        }

        public void Receive(string queueName, string exchangeName, string exchangeType, Action<string> func, string routingKey = "")
        {
            Factory.DispatchConsumersAsync = false;
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, exchangeType, true);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);
            //consume one by one
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    func(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                }

            };
            //manually confirm
            channel.BasicConsume(queueName, false, consumer);
        }
        public void ReceiveAsync(string queueName, string exchangeName, string exchangeType, Func<string, Task> func, string routingKey = "")
        {
            Factory.DispatchConsumersAsync = true;
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, exchangeType, true);
            channel.QueueDeclare(queueName, true, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey);
            //consume one by one
            channel.BasicQos(0, 1, false);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    await func(message);
                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicReject(ea.DeliveryTag, true);
                    throw;
                }

            };
            //manually confirm
            channel.BasicConsume(queueName, false, consumer);
        }
    }
}
