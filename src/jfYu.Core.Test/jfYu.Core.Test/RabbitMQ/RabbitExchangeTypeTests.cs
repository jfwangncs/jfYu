using jfYu.Core.RabbitMQ;
using jfYu.Core.Test.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace jfYu.Core.Test.RabbitMQ
{
    [Collection("RabbitMQ")]
    public class RabbitExchangeTypeTests
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IModel _channel;

        public RabbitExchangeTypeTests(IRabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
            _channel = _rabbitMQService.Connection.CreateModel();
        }

        public class NullStringExpectData : TheoryData<string, string>
        {
            public NullStringExpectData()
            {
                Add("2", "");
                Add("3", "    ");
                Add("4", "This is message");
            }
        }

        [Fact]
        public void AddRabbitMQService_ThrowException()
        {
            var services = new ServiceCollection();
            Assert.Throws<ArgumentNullException>(() => services.AddRabbitMQService(null!));
        }

        [Theory]
        [ClassData(typeof(NullStringExpectData))]
        public async Task Send_String(string index, string message)
        {
            string exchangeName = $"Send_String{index}";
            string queueName = $"Send_String{index}";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);

            _rabbitMQService.Send(exchangeName, message);

            var receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(message, receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_Model()
        {
            const string exchangeName = "Send_Model";
            const string queueName = "Send_Model";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);

            var message = new TestModelFaker().Generate(12);
            _rabbitMQService.Send(exchangeName, message);

            var receivedMessages = new List<TestModel>();
            _rabbitMQService.Receive<List<TestModel>>(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(JsonConvert.SerializeObject(message), JsonConvert.SerializeObject(receivedMessages));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_DirectExchange()
        {
            const string exchangeName = "direct_exchange";
            const string queueName = "direct_queue";
            const string routingKey = "error";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct, routingKey);

            string message = "This is an error message";
            _rabbitMQService.Send(exchangeName, message, routingKey);

            var receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(message, receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_DirectExchangePrefetchCount()
        {
            const string exchangeName = "direct_exchange_async";
            const string queueName = "direct_queue_async";
            const string routingKey = "error";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct, routingKey);

            string message = "This is an error message";
            _rabbitMQService.Send(exchangeName, message, routingKey);
            _rabbitMQService.Send(exchangeName, message, routingKey);
            _rabbitMQService.Send(exchangeName, message, routingKey);
            _rabbitMQService.Send(exchangeName, message, routingKey);
            var sendMessages = new List<string> { message, message, message, message };
            var receivedMessages = new List<string>();
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages.Add(q);
                return true;
            }, 5);

            await Task.Delay(1000);
            Assert.Equal(JsonConvert.SerializeObject(sendMessages), JsonConvert.SerializeObject(receivedMessages));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_FanoutExchange()
        {
            const string exchangeName = "fanout_exchange";
            const string queue1 = "fanout_queue1";
            const string queue2 = "fanout_queue2";

            _rabbitMQService.QueueBind(queue1, exchangeName, ExchangeType.Fanout);
            _rabbitMQService.QueueBind(queue2, exchangeName, ExchangeType.Fanout);

            string message = "Broadcast message";
            _rabbitMQService.Send(exchangeName, message);

            var receivedMessagesQueue1 = "";
            var receivedMessagesQueue2 = "";

            _rabbitMQService.Receive(queue1, q =>
            {
                receivedMessagesQueue1 = q;
                return true;
            });
            _rabbitMQService.Receive(queue2, q =>
            {
                receivedMessagesQueue2 = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(message, receivedMessagesQueue1);
            Assert.Equal(message, receivedMessagesQueue2);
            _channel.QueueDelete(queue1);
            _channel.QueueDelete(queue2);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_HeadersExchange()
        {
            const string exchangeName = "headers_exchange";
            const string queueName = "headers_queue";

            var headers = new Dictionary<string, object>
        {
            { "x-match", "all" },
                { "type", "error" },
            { "format", "json" }
        };
            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Headers, "", headers);

            string message = "Headers matched message";
            _rabbitMQService.Send(exchangeName, message, "", headers);

            var receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(message, receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Test_TopicExchange()
        {
            const string exchangeName = "topic_exchange";
            const string queueName1 = "topic_queue1";
            const string queueName2 = "topic_queue2";

            _rabbitMQService.QueueBind(queueName1, exchangeName, ExchangeType.Topic, "logs.error.#");
            _rabbitMQService.QueueBind(queueName2, exchangeName, ExchangeType.Topic, "logs.*.database");

            var receivedMessagesQueue1 = new List<string>();
            var receivedMessagesQueue2 = new List<string>();

            _rabbitMQService.Receive(queueName1, q =>
            {
                receivedMessagesQueue1.Add(q);
                return true;
            });

            _rabbitMQService.Receive(queueName2, q =>
            {
                receivedMessagesQueue2.Add(q);
                return true;
            });

            string message1 = "Error in database";
            _rabbitMQService.Send(exchangeName, message1, "logs.error.database");

            string message2 = "Info in database";
            _rabbitMQService.Send(exchangeName, message2, "logs.info.database");

            await Task.Delay(500);
            Assert.Contains(message1, receivedMessagesQueue1);
            Assert.Contains(message1, receivedMessagesQueue2);
            Assert.DoesNotContain(message2, receivedMessagesQueue1);
            Assert.Contains(message2, receivedMessagesQueue2);

            _channel.QueueDelete(queueName1);
            _channel.QueueDelete(queueName2);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Send_MultipleExchange()
        {
            const string exchangeName1 = "direct_exchange1";
            const string exchangeName2 = "direct_exchange2";
            const string queueName = "direct_multiplequeue";

            _rabbitMQService.QueueBind(queueName, exchangeName1, ExchangeType.Direct);
            _rabbitMQService.ExchangeBind(exchangeName1, exchangeName2, ExchangeType.Direct);

            string message = "This is an error message";
            _rabbitMQService.Send(exchangeName2, message);

            var receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                receivedMessages = q;
                return true;
            });

            await Task.Delay(500);
            Assert.Equal(message, receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName1);
            _channel.ExchangeDelete(exchangeName2);
        }

        [Fact]
        public async Task Receive_RejectFirst()
        {
            const string exchangeName = "reject_exchange";
            const string queueName = "reject_queue";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);

            string message = "This is an error message";
            _rabbitMQService.Send(exchangeName, message);

            int i = 0;
            var receivedMessages = "";
            _rabbitMQService.Receive(queueName, q =>
            {
                if (i == 0)
                {
                    i++;
                    return false;
                }
                receivedMessages = q;
                return true;
            });

            await Task.Delay(1000);
            Assert.Equal(1, i);
            Assert.Equal(message, receivedMessages);
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }

        [Fact]
        public async Task Receive_RejectModelFirst()
        {
            const string exchangeName = "reject_exchange_model";
            const string queueName = "reject_queue_model";

            _rabbitMQService.QueueBind(queueName, exchangeName, ExchangeType.Direct);

            var message = new TestModelFaker().Generate(1).FirstOrDefault();

            _rabbitMQService.Send(exchangeName, message);

            int i = 0;
            var receivedMessages = new TestModel();
            _rabbitMQService.Receive<TestModel>(queueName, q =>
            {
                if (i == 0)
                {
                    i++;
                    return false;
                }
                receivedMessages = q;
                return true;
            });

            await Task.Delay(1000);
            Assert.Equal(1, i);
            Assert.Equal(JsonConvert.SerializeObject(message), JsonConvert.SerializeObject(receivedMessages));
            _channel.QueueDelete(queueName);
            _channel.ExchangeDelete(exchangeName);
        }
    }
}