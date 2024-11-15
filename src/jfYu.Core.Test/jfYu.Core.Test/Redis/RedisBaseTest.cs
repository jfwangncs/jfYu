using jfYu.Core.Redis;
using jfYu.Core.Redis.Implementation;
using jfYu.Core.Redis.Interface;
using jfYu.Core.Redis.Serializer;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisBaseTest
    {
        [Fact]
        public void AddRedisService_WhenSetupActionIsNull_ThrowsException()
        {
            var services = new ServiceCollection();
            Assert.Throws<ArgumentNullException>(() => services.AddRedisService(default));
        }

        [Fact]
        public void AddRedisService_WhenEndpointsAreEmpty_ThrowsException()
        {
            var services = new ServiceCollection();

            Assert.Throws<NullReferenceException>(() => services.AddRedisService(options =>
            {
                options.EndPoints = []; // Empty list
            }));
        }

        [Fact]
        public void AddRedisService_WhenEndpointsAreNotConfigured_ThrowsException()
        {
            var services = new ServiceCollection();

            var ex = Record.Exception(() => services.AddRedisService(options =>
            {
                // No endpoints configured
            }));

            Assert.IsType<NullReferenceException>(ex);
        }       

        [Fact]
        public void AddRedisService_RegistersConnectionMultiplexerAndRedisService()
        {
            var services = new ServiceCollection();
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.Timeout = 5000;
                options.Ssl = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var connectionMultiplexer = serviceProvider.GetService<IConnectionMultiplexer>();
            var redisService = serviceProvider.GetService<IRedisService>();

            Assert.NotNull(connectionMultiplexer);
            Assert.NotNull(redisService);
            Assert.IsType<RedisService>(redisService);
        }
        [Fact]
        public void UsingNewtonsoft_RegistersNewtonsoftSerializer()
        {
            var services = new ServiceCollection();
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.Password = "password";
                options.Timeout = 5000;
                options.Ssl = false;
            }).UsingNewtonsoft();

            var serviceProvider = services.BuildServiceProvider();

            var serializer = serviceProvider.GetService<ISerializer>();

            Assert.NotNull(serializer);
            Assert.IsType<NewtonsoftSerializer>(serializer);
        }       

        [Fact]
        public void UsingMsgPack_RegistersMsgPackSerializer()
        {
            var services = new ServiceCollection();
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.Password = "password";
                options.Timeout = 5000;
                options.Ssl = false;
            }).UsingMsgPack();

            var serviceProvider = services.BuildServiceProvider();

            var serializer = serviceProvider.GetService<ISerializer>();

            Assert.NotNull(serializer);
            Assert.IsType<MsgPackObjectSerializer>(serializer);
        }

        [Fact]
        public void DefaultUsesNewtonsoft_WhenNoSerializerSpecified()
        {
            var services = new ServiceCollection();
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.Password = "password";
                options.Timeout = 5000;
                options.Ssl = false;
            });

            var serviceProvider = services.BuildServiceProvider();

            var serializer = serviceProvider.GetService<ISerializer>();

            Assert.NotNull(serializer);
            Assert.IsType<NewtonsoftSerializer>(serializer);
        }
    }
}
