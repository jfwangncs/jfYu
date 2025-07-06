using BenchmarkDotNet.Attributes;
using JfYu.RabbitMQ;
using JfYu.UnitTests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;


namespace JfYu.UnitTests.RabbitMQ
{ 
    [MemoryDiagnoser]
    public class SendAsyncBenchmark
    {
        private IRabbitMQService _rabbitMQService = null!;
        private readonly string _exchangeName = "benchmark_exchange";
        private readonly string _queueName = "benchmark_queue";
        private List<TestModel> _messages = [];

        [Params(1, 10, 100)]
        public int BatchSize;

        [GlobalSetup]
        public async Task Setup()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                  .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                  .Build();

            services.AddRabbitMQ((q, e) =>
            {
                configuration.GetSection("RabbitMQ").Bind(q);
                configuration.GetSection("RabbitMQ:MessageOption").Bind(e);
            });
            var serviceProvider = services.BuildServiceProvider();
            _rabbitMQService = serviceProvider.GetRequiredService<IRabbitMQService>();

            await _rabbitMQService.QueueDeclareAsync(_queueName, _exchangeName);
            _messages = new TestModelFaker().Generate(10000);
        }

        [Benchmark]
        public async Task SendAsync()
        {
            for (int i = 0; i < BatchSize; i++)
            {
                await _rabbitMQService.SendAsync(_exchangeName, _messages[i]);
            }
        }
        [Benchmark]
        public async Task SendBatchAsync_String()
        {
            var batch = _messages.Take(BatchSize).ToList();
            await _rabbitMQService.SendBatchAsync(_exchangeName, batch);
        }

        [GlobalCleanup]
        public async Task Cleanup()
        {
            var channel = await _rabbitMQService.Connection.CreateChannelAsync();
            await channel.QueueDeleteAsync(_queueName);
            await channel.ExchangeDeleteAsync(_exchangeName);
            await channel.CloseAsync();
            await channel.DisposeAsync();
        }
    }
}
