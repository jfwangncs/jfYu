using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JfYu.RabbitMQ
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="messageRetryPolicy"></param>
    /// <param name="logger"></param>
    public class RabbitMQService(IConnection connection, MessageOptions messageRetryPolicy, ILogger<RabbitMQService>? logger = null) : IRabbitMQService
    {
        /// <summary>
        /// Rabbit MQ connection.
        /// </summary>
        public IConnection Connection { get; } = connection;

        private const string xretrycount = "x-retry-count";
        private const string xexchangename = "x-exchange-name";
        private const string xexchangeroutingkey = "x-exchange-routing-key";
        private const string xdeadletterexchange = "x-dead-letter-exchange";
        private const string xdeadletterroutingkey = "x-dead-letter-routing-key";


        private const string receiveError = "Receive message have error.message:{message}";
        private readonly MessageOptions _messageRetryPolicy = messageRetryPolicy;
        private readonly ILogger<RabbitMQService>? _logger = logger;

        /// <inheritdoc/>
        public async Task QueueBindAsync(string queueName, string exchangeName, string exchangeType, string routingKey = "", Dictionary<string, object?>? headers = null)
        {
            using var _channel = await Connection.CreateChannelAsync().ConfigureAwait(false);
            await _channel.ExchangeDeclareAsync(exchangeName, exchangeType, true).ConfigureAwait(false);
            await _channel.QueueDeclareAsync(queueName, true, false, false, null).ConfigureAwait(false);
            await _channel.QueueBindAsync(queueName, exchangeName, routingKey, headers).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task ExchangeBindAsync(string destination, string source, string exchangeType, string routingKey = "", Dictionary<string, object?>? headers = null)
        {
            using var _channel = await Connection.CreateChannelAsync().ConfigureAwait(false);
            await _channel.ExchangeDeclareAsync(destination, exchangeType, true).ConfigureAwait(false);
            await _channel.ExchangeDeclareAsync(source, exchangeType, true).ConfigureAwait(false);
            await _channel.ExchangeBindAsync(destination, source, routingKey, headers).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task Send<T>(string exchangeName, List<T> msgs, string routingKey = "", Dictionary<string, object?>? headers = null)
        {
            using var limiter = new ThrottlingRateLimiter(_messageRetryPolicy.MaxOutstandingConfirms);
            var channelOpts = new CreateChannelOptions(true, true, limiter);
            using var _channel = await Connection.CreateChannelAsync(channelOpts).ConfigureAwait(false);
            headers ??= [];
            headers.TryAdd(xretrycount, 0);
            headers.TryAdd(xexchangename, exchangeName);
            headers.TryAdd(xexchangeroutingkey, routingKey);            
            var basicProperties = new BasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = headers;
            var publishTasks = new List<ValueTask>();
            for (int i = 0; i < msgs.Count; i++)
            {
                var msg = msgs[i];
                byte[] payload;
                if (msg is string str)
                    payload = Encoding.UTF8.GetBytes(str);
                else
                    payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));

                ValueTask publishTask = _channel.BasicPublishAsync(exchangeName, routingKey, true, basicProperties, payload);
                publishTasks.Add(publishTask);

                await Publishes(publishTasks, _messageRetryPolicy.BatchSize).ConfigureAwait(false);
            }

            // Await any remaining tasks in case message count was not
            // evenly divisible by batch size.
            await Publishes(publishTasks, 0).ConfigureAwait(false);

            async Task Publishes(List<ValueTask> publishTasks, int batchSize)
            {
                if (publishTasks.Count >= batchSize)
                {
                    foreach (ValueTask pt in publishTasks)
                    {
                        try
                        {
                            await pt.ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            _logger?.LogError(ex, receiveError, "");
                        }
                    }
                    publishTasks.Clear();
                }
            }
        }

        /// <inheritdoc/>
        public async IChannel Receive<T>(string queueName, Func<T?, Task<bool>> func, ushort prefetchCount = 1)
        {
            var _channel = await connection.CreateChannelAsync().ConfigureAwait(false);
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false).ConfigureAwait(false);
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (ch, ea) =>
            {
                string message = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    T? obj = typeof(T) == typeof(string) ? (T?)(object)message : JsonConvert.DeserializeObject<T>(message);

                    if (await func(obj).ConfigureAwait(false))
                        await _channel.BasicAckAsync(ea.DeliveryTag, false).ConfigureAwait(false);
                    else
                    {
                        if (_messageRetryPolicy.EnableDeadQueue)
                            await _channel.BasicRejectAsync(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(ea)).ConfigureAwait(false);
                        else
                            await _channel.BasicRejectAsync(ea.DeliveryTag, true).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, receiveError, message);
                    if (_messageRetryPolicy.EnableDeadQueue)
                        await _channel.BasicRejectAsync(ea.DeliveryTag, !TryToMoveToDeadLetterQueue(ea)).ConfigureAwait(false);
                    else
                        await _channel.BasicRejectAsync(ea.DeliveryTag, true).ConfigureAwait(false);
                }
            };
            AddModelShutdownEvent(_channel);
            await _channel.BasicConsumeAsync(queueName, false, consumer).ConfigureAwait(false);
            return _channel;
        }

        /// <inheritdoc/>
        private bool TryToMoveToDeadLetterQueue(BasicDeliverEventArgs ea)
        {
            if (ea.BasicProperties.Headers is null)
                return false;

            try
            {
                using var channel = Connection.CreateModel();
                channel.ConfirmSelect();
                int retryCount = -1;
                var originalRoutingKey = "";
                var originalExchangeName = "";
                if (ea.BasicProperties.Headers.TryGetValue(xretrycount, out object? value))
                    retryCount = Convert.ToInt32(value);
                if (ea.BasicProperties.Headers.TryGetValue(xexchangename, out object? value1))
                    originalExchangeName = Encoding.UTF8.GetString((byte[])value1);
                if (ea.BasicProperties.Headers.TryGetValue(xexchangeroutingkey, out object? value2))
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
                    ea.BasicProperties.Headers[xretrycount] = retryCount;
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