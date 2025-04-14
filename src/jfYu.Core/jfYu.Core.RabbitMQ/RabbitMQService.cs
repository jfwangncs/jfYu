using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace jfYu.Core.RabbitMQ
{

    /// <summary>
    ///  
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="messageRetryPolicy"></param>
    /// <param name="logger"></param>
    public class RabbitMQService(IConnection connection, MessageRetryPolicy messageRetryPolicy, ILogger<RabbitMQService>? logger = null) : IRabbitMQService
    {
        /// <summary>
        /// Rabbit MQ connection.
        /// </summary>
        public IConnection Connection { get; } = connection;

        private readonly MessageRetryPolicy _messageRetryPolicy = messageRetryPolicy;
        private readonly ILogger<RabbitMQService>? _logger = logger;



        /// <inheritdoc/>
        public bool QueueBind(string queueName, string exchangeName, string exchangeType, string routingKey = "", Dictionary<string, object>? headers = null)
        {
           var _channel = Connection.CreateModel();
            _channel.ConfirmSelect(); // confirm
            _channel.ExchangeDeclare(exchangeName, exchangeType, true);
            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, exchangeName, routingKey, headers);
            return _channel.WaitForConfirms(_channel.ContinuationTimeout);
        }


        /// <inheritdoc/>

        public bool ExchangeBind(string destination, string source, string exchangeType, string routingKey = "", Dictionary<string, object>? headers = null)
        {
            var _channel = Connection.CreateModel();
            _channel.ConfirmSelect(); // confirm
            _channel.ExchangeDeclare(destination, exchangeType, true);
            _channel.ExchangeDeclare(source, exchangeType, true);
            _channel.ExchangeBind(destination, source, routingKey, headers);
            return _channel.WaitForConfirms(_channel.ContinuationTimeout);
        }

        /// <inheritdoc/>
        public bool Send(string exchangeName, string msg, string routingKey = "", Dictionary<string, object>? headers = null)
        {
            var _channel = Connection.CreateModel();
            headers ??= [];
            headers.TryAdd("x-retry-count", 0);
            headers.TryAdd("x-exchange-name", exchangeName);
            headers.TryAdd("x-exchange-routing-key", routingKey);
            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = headers;
            var payload = Encoding.UTF8.GetBytes(msg);
            _channel.BasicPublish(exchangeName, routingKey, basicProperties, payload);
            return _channel.WaitForConfirms(_channel.ContinuationTimeout);
        }

        /// <inheritdoc/>
        public bool Send<T>(string exchangeName, T msg, string routingKey = "", Dictionary<string, object>? headers = null)
        {
            var _channel = Connection.CreateModel();
            headers ??= [];
            headers.TryAdd("x-retry-count", 0);
            headers.TryAdd("x-exchange-name", exchangeName);
            headers.TryAdd("x-exchange-routing-key", routingKey);
            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = headers;
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
            _channel.BasicPublish(exchangeName, routingKey, basicProperties, payload);
            return _channel.WaitForConfirms(_channel.ContinuationTimeout);
        }

        /// <inheritdoc/>
        public void Receive(string queueName, Func<string, bool> func, ushort prefetchCount = 1)
        {
            var _channel = Connection.CreateModel();
            _channel.BasicQos(0, prefetchCount, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    if (func(message))
                        _channel.BasicAck(ea.DeliveryTag, false);
                    else
                        if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Receive message have error.");
                    if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                ;
            };
            _channel.BasicConsume(queueName, false, consumer);
        }


        /// <inheritdoc/>
        public void Receive(string queueName, Func<string, Task<bool>> func, ushort prefetchCount = 1)
        {
            var _channel = Connection.CreateModel();
            _channel.BasicQos(0, prefetchCount, false);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    if (await func(message))
                        _channel.BasicAck(ea.DeliveryTag, false);
                    else
                      if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Receive message have error.");
                    if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                ;

            };
            _channel.BasicConsume(queueName, false, consumer);
        }

        /// <inheritdoc/>
        public void Receive<T>(string queueName, Func<T?, bool> func, ushort prefetchCount = 1)
        {
            var _channel = Connection.CreateModel();
            _channel.BasicQos(0, prefetchCount, false);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {

                try
                {

                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    if (func(JsonConvert.DeserializeObject<T>(message)))
                        _channel.BasicAck(ea.DeliveryTag, false);
                    else
                        if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Receive message have error.");
                    if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                ;
            };
            _channel.BasicConsume(queueName, false, consumer);
        }


        /// <inheritdoc/>
        public void Receive<T>(string queueName, Func<T?, Task<bool>> func, ushort prefetchCount = 1)
        {
            var _channel = Connection.CreateModel();
            _channel.BasicQos(0, prefetchCount, false);
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {

                try
                {
                    string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    if (await func(JsonConvert.DeserializeObject<T>(message)))
                        _channel.BasicAck(ea.DeliveryTag, false);
                    else
                         if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Receive message have error.");
                    if (_messageRetryPolicy.EnableDeadQueue)
                        _channel.BasicReject(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(_channel,ea));
                    else
                        _channel.BasicReject(ea.DeliveryTag, true);
                }
                ;

            };
            _channel.BasicConsume(queueName, false, consumer);
        }


        /// <inheritdoc/>
        private bool TryToMoveToDeadLetterQueue(IModel channel, BasicDeliverEventArgs ea)
        { 
            if (ea.BasicProperties.Headers is null)
                return false;

            try
            {

                int retryCount = -1;
                var originalRoutingKey = "";
                var originalExchangeName = "";
                if (ea.BasicProperties.Headers.TryGetValue("x-retry-count", out object? value))
                    retryCount = Convert.ToInt32(value);
                if (ea.BasicProperties.Headers.TryGetValue("x-exchange-name", out object? value1))
                    originalExchangeName = Encoding.UTF8.GetString((byte[])value1);
                if (ea.BasicProperties.Headers.TryGetValue("x-exchange-routing-key", out object? value2))
                    originalRoutingKey = Encoding.UTF8.GetString((byte[])value2);
                if (retryCount == -1 || string.IsNullOrEmpty(originalExchangeName))
                {
                    _logger?.LogWarning("Message didn't have x-retry-count,x-exchange-name,x-exchange-routing-key can't use dead letter queue.");
                    return false;
                }
                if (retryCount >= _messageRetryPolicy.MaxRetryCount)
                {
                    //send to dead letter queue
                    _logger?.LogInformation("Message tried to exceed the retry limit:{maxRetryCount}，send it to dead queue:{queueName}.", _messageRetryPolicy.MaxRetryCount, _messageRetryPolicy.DeadLetterQueue);
                    var basicProperties = channel.CreateBasicProperties();
                    basicProperties.Persistent = true;
                    basicProperties.Headers = ea.BasicProperties.Headers;
                    basicProperties.Headers["x-original-routing-key"] = ea.RoutingKey;
                    channel.BasicPublish(_messageRetryPolicy.DeadLetterExchange, "", basicProperties, ea.Body);
                    return channel.WaitForConfirms(channel.ContinuationTimeout);
                }
                else
                {
                    //retry message 
                    retryCount++;
                    ea.BasicProperties.Headers["x-retry-count"] = retryCount;
                    channel.BasicPublish(originalExchangeName, originalRoutingKey, ea.BasicProperties, ea.Body);
                    return channel.WaitForConfirms(channel.ContinuationTimeout);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
