using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Text;
using JfYu.RabbitMQ;
using RabbitMQ.Client;

namespace JfYu.UnitTests.RabbitMQ
{
    public class RabbitDeadQueueTests
    {
        #region ReceiveT

        [Fact]
        public async Task Receive_ThrowException_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(Receive_ThrowException_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(Receive_ThrowException_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQ((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                 
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<string>(queueName, q =>
            {
                i++;
                receivedMessages = q;
                return true;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(10));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task Receive_ReturnFalse_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(Receive_ReturnFalse_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(Receive_ReturnFalse_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            Assert.InRange(i, 10, 10000);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Receive_ThrowException_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_ThrowException_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_ThrowException_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(Receive_ThrowException_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(Receive_ThrowException_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<string>(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, q =>
            {
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }
             
        [Fact]
        public async Task Receive_RetureFalse_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_RetureFalse_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_RetureFalse_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(Receive_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(Receive_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                return false;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, q =>
            {
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        public class HeaderHaveErrorExpectData : TheoryData<Dictionary<string, object>?>
        {
            public HeaderHaveErrorExpectData()
            {
                Add(null);
                Add([]);
                Add(new Dictionary<string, object>() { { "x-retry-count", 0 } });
                Add(new Dictionary<string, object>() { { "x-exchange-name", "test" } });
                Add(new Dictionary<string, object>() { { "x-retry-count", "1xxx" } });
            }
        }

        [Theory]
        [ClassData(typeof(HeaderHaveErrorExpectData))]
        public async Task Receive_HeaderHaveError_DeadQueueEnabled(Dictionary<string, object> headers)
        {
            const string exchangeName = $"{nameof(Receive_HeaderHaveError_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_HeaderHaveError_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(Receive_HeaderHaveError_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(Receive_HeaderHaveError_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);
            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();

            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, q =>
            {
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = headers;
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchangeName, "", basicProperties, payload);
            _channel.WaitForConfirms();

            await Task.Delay(2000);
            Assert.Empty(receivedMessages);
            Assert.InRange(i, 1, 10000);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task Receive_WarnWithNullLog_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_WarnWithNullLog_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_WarnWithNullLog_DeadQueueEnabled)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();

            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = new Dictionary<string, object>() { { "x-retry-count", -1 } };
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchangeName, "", basicProperties, payload);
            _channel.WaitForConfirms();

            await Task.Delay(500);
            Assert.Empty(receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task Receive_WithLogWarn_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_WithLogWarn_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_WithLogWarn_DeadQueueEnabled)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
            });

            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);
            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();

            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = new Dictionary<string, object>() { { "x-retry-count", -1 } };
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchangeName, "", basicProperties, payload);
            _channel.WaitForConfirms();

            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Receive_ErrorWithNullLog_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_ErrorWithNullLog_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_ErrorWithNullLog_DeadQueueEnabled)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();

            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = new Dictionary<string, object>() { { "x-retry-count", "dada" } };
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchangeName, "", basicProperties, payload);
            _channel.WaitForConfirms();

            await Task.Delay(500);
            Assert.Empty(receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task Receive_WithLogError_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(Receive_WithLogWarn_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(Receive_WithLogWarn_DeadQueueEnabled)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
            });

            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);
            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();

            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                i += 1;
                return false;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _channel.ConfirmSelect();
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Persistent = true;
            basicProperties.Headers = new Dictionary<string, object>() { { "x-retry-count", "dada" } };
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish(exchangeName, "", basicProperties, payload);
            _channel.WaitForConfirms();

            await Task.Delay(2000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task Receive_ChannelShutDown_LogEnabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(Receive_ChannelShutDown_LogEnabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(Receive_ChannelShutDown_LogEnabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQ((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<string>(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });
            _rabbitMQService.Send(exchangeName, JsonConvert.SerializeObject("test"));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Equal("test", receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Channel have shutdown")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
        }
        [Fact]
        public async Task Receive_ChannelShutDown_LogDisabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(Receive_ChannelShutDown_LogDisabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(Receive_ChannelShutDown_LogDisabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQ((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<string>(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });
            _rabbitMQService.Send(exchangeName, JsonConvert.SerializeObject("test"));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Equal("test", receivedMessages);
        }

        #endregion ReceiveT

        #region ReceiveString

        [Fact]
        public async Task ReceiveString_ThrowException_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveString_ThrowException_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_ThrowException_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = false;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                int x = 0;
                var y = 1 / x;
                return true;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(10));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveString_ReturnFalse_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveString_ReturnFalse_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_ReturnFalse_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive(queueName, q =>
            {
                i += 1;
                return false;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            Assert.InRange(i, 10, 10000);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveString_ThrowException_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveString_ThrowException_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_ThrowException_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveString_ThrowException_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveString_ThrowException_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                int x = 0;
                var y = 1 / x;
                return true;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, q =>
            {
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task ReceiveString_RetureFalse_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveString_RetureFalse_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_RetureFalse_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveString_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveString_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                return false;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, q =>
            {
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task ReceiveString_ChannelShutDown_LogEnabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveString_ChannelShutDown_LogEnabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_ChannelShutDown_LogEnabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQ((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Equal("test", receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Channel have shutdown")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
        }

        [Fact]
        public async Task ReceiveString_ChannelShutDown_LogDisabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveString_ChannelShutDown_LogDisabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveString_ChannelShutDown_LogDisabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQ((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Equal("test", receivedMessages);
        }

        #endregion ReceiveString

        #region ReceiveAsync

        [Fact]
        public async Task ReceiveAsync_ThrowException_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_ThrowException_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_ThrowException_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = false;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<TestModel>(queueName, async q =>
            {
                await Task.FromException(new Exception(""));
                return true;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveAsync_ReturnFalse_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_ReturnFalse_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_ReturnFalse_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive<TestModel>(queueName, async q =>
            {
                i += 1;
                return await Task.FromResult(false);
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            Assert.InRange(i, 10, 10000);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveAsync_ThrowException_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_ThrowException_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_ThrowException_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveAsync_ThrowException_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveAsync_ThrowException_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<TestModel>(queueName, async q =>
            {
                await Task.FromException(new Exception(""));
                return true;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, async q =>
            {
                await Task.Delay(1);
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task ReceiveAsync_RetureFalse_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_RetureFalse_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_RetureFalse_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveAsync_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveAsync_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<TestModel>(queueName, async q =>
            {
                return await Task.FromResult(false);
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, async q =>
            {
                await Task.Delay(1);
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task ReceiveAsync_Successful()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_Successful)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_Successful)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            TestModel? receivedMessages = null;
            _rabbitMQService.Receive<TestModel>(queueName, async q =>
            {
                await Task.Delay(1);
                receivedMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Equal(JsonConvert.SerializeObject(message), JsonConvert.SerializeObject(receivedMessages));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task ReceiveAsync_ChannelShutDown_LogEnabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_ChannelShutDown_LogEnabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_ChannelShutDown_LogEnabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
                q.DispatchConsumersAsync = true;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<string>(queueName, async q =>
            {
                await Task.Delay(100);
                return false;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Channel have shutdown")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
        }

        [Fact]
        public async Task ReceiveAsync_ChannelShutDown_LogDisabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveAsync_ChannelShutDown_LogDisabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsync_ChannelShutDown_LogDisabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
                q.DispatchConsumersAsync = true;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive<string>(queueName, async q =>
            {
                await Task.Delay(100);
                return false;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
        }

        #endregion ReceiveAsync

        #region ReceiveAsync

        [Fact]
        public async Task ReceiveAsyncString_ThrowException_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_ThrowException_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_ThrowException_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = false;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, async q =>
            {
                await Task.FromException(new Exception(""));
                return true;
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeastOnce);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveAsyncString_ReturnFalse_ButDeadQueueEnableFalse()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_ReturnFalse_ButDeadQueueEnableFalse)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_ReturnFalse_ButDeadQueueEnableFalse)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            int i = 0;
            _rabbitMQService.Receive(queueName, async q =>
            {
                i += 1;
                return await Task.FromResult(false);
            });

            var message = new TestModelFaker().Generate(1).FirstOrDefault();
            _rabbitMQService.Send(exchangeName, message);
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            Assert.InRange(i, 10, 10000);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName); ;
        }

        [Fact]
        public async Task ReceiveAsyncString_ThrowException_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_ThrowException_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_ThrowException_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveAsyncString_ThrowException_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveAsyncString_ThrowException_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, async q =>
            {
                await Task.FromException(new Exception(""));
                return true;
            });

            string? receivedStringMessages = "";
            _rabbitMQService.Receive(deadLetterQueue, async q =>
            {
                await Task.Delay(1);
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), receivedStringMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }

        [Fact]
        public async Task ReceiveAsyncString_RetureFalse_DeadQueueEnabled()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_RetureFalse_DeadQueueEnabled)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_RetureFalse_DeadQueueEnabled)}_Queue";
            const string deadLetterQueue = $"{nameof(ReceiveAsyncString_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Queue";
            const string deadLetterExchange = $"{nameof(ReceiveAsyncString_RetureFalse_DeadQueueEnabled)}_Dead_Letter_Exchange";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();
            var retryPolicy = configuration.GetRequiredSection("RabbitMQ:RetryPolicy").Get<MessageRetryPolicy>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                q.DispatchConsumersAsync = true;

                e.EnableDeadQueue = true;
                e.RetryDelayMilliseconds = 1000;
                e.DeadLetterQueue = deadLetterQueue;
                e.DeadLetterExchange = deadLetterExchange;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            var _retry = serviceProvider.GetRequiredService<MessageRetryPolicy>();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            _rabbitMQService.QueueBind(_retry.DeadLetterQueue, _retry.DeadLetterExchange, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, async q =>
            {
                await Task.Delay(100);
                return false;
            });

            TestModel? receivedStringMessages = null;
            _rabbitMQService.Receive<TestModel>(deadLetterQueue, async q =>
            {
                await Task.Delay(1);
                receivedStringMessages = q;
                return true;
            });
            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            await Task.Delay(10000);
            Assert.Empty(receivedMessages);
            Assert.Equal(JsonConvert.SerializeObject(message), JsonConvert.SerializeObject(receivedStringMessages));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _channel.QueueDelete(deadLetterQueue);
            _channel.ExchangeDelete(deadLetterExchange);
        }
        [Fact]
        public async Task ReceiveAsyncString_ChannelShutDown_LogEnabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_ChannelShutDown_LogEnabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_ChannelShutDown_LogEnabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
                q.DispatchConsumersAsync = true;
            });
            var mockLogger = new Mock<ILogger<RabbitMQService>>();

            services.AddSingleton(mockLogger.Object);

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, async q =>
            {
                await Task.Delay(100);
                return false;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
            mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => (v.ToString() ?? "").Contains("Channel have shutdown")), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.AtLeast(1));
        }

        [Fact]
        public async Task ReceiveAsyncString_ChannelShutDown_LogDisabled_LogCorrectly()
        {
            const string exchangeName = $"{nameof(ReceiveAsyncString_ChannelShutDown_LogDisabled_LogCorrectly)}_Exchange";
            const string queueName = $"{nameof(ReceiveAsyncString_ChannelShutDown_LogDisabled_LogCorrectly)}_Queue";

            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            var rabbitConfig = configuration.GetRequiredSection("RabbitMQ").Get<ConnectionFactory>();

            var services = new ServiceCollection();
            services.AddRabbitMQService((q, e) =>
            {
                q.HostName = rabbitConfig!.HostName;
                q.Port = rabbitConfig.Port;
                q.UserName = rabbitConfig.UserName;
                q.Password = rabbitConfig.Password;
                e.RetryDelayMilliseconds = 1000;
                q.DispatchConsumersAsync = true;
            });

            var serviceProvider = services.BuildServiceProvider();
            var _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();
            var _channel = _rabbitMQService.Connection.CreateModel();
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);
            string? receivedMessages = "";
            _rabbitMQService.Receive(queueName, async q =>
            {
                await Task.Delay(100);
                return false;
            });
            _rabbitMQService.Send(exchangeName, "test");
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
            _rabbitMQService.Connection.Dispose();
            await Task.Delay(1000);
            Assert.Empty(receivedMessages);
        }
        #endregion ReceiveAsync
    }
}