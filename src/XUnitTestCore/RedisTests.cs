using jfYu.Core.Redis;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore
{
    public class RedisTest
    {
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Address { get; set; }
    }


    public class RedisTests
    {
        public readonly IRedisService _redis;

        public RedisTests()
        {
            var dic = new List<Tuple<string, string, DateTime?>>();
            var databaseMock = new Mock<IDatabase>();
            databaseMock.Setup(q => q.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<bool>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
                .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>((k, v, expiry, x, x1, x2) =>
                {
                    var date = DateTime.Now;
                    if (expiry == null)
                        date = date.AddDays(10);
                    else
                        date = date.Add(expiry.Value);
                    dic.Add(new Tuple<string, string, DateTime?>(k.ToString(), v.ToString(), date));
                }).ReturnsAsync(true);
            databaseMock.Setup(q => q.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync((RedisKey q, CommandFlags e) =>
            {
                var x = q.ToString();
                var t = dic.FirstOrDefault(q => q.Item1.Equals(x));
                if (t == null)
                    return RedisValue.Null;
                if (t.Item3 < DateTime.Now)
                    return RedisValue.Null;
                else
                    return t.Item2;
            });

            databaseMock.Setup(q => q.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync((RedisKey q, CommandFlags e) =>
            {
                var x = q.ToString();
                var t = dic.FirstOrDefault(q => q.Item1.Equals(x));
                if (t != null)
                    dic.Remove(t);
                return true;

            });
            databaseMock.Setup(d => d.LockTakeAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
            databaseMock.Setup(d => d.LockReleaseAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);
            var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
            connectionMultiplexerMock.Setup(q => q.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(databaseMock.Object);

            var redisMock = new RedisService(new RedisConfiguration() { EndPoints = [new RedisEndPoint() { Host = "127.0.0.1" }] }, connectionMultiplexerMock.Object)
            {
                Client = connectionMultiplexerMock.Object
            };

            _redis = redisMock;
        }

        [Fact]
        public void TestCreateEmpty()
        {
            var services = new ServiceCollection();
            Assert.ThrowsAny<Exception>(() => { services.AddRedisService(new RedisConfiguration() { }); });
            //services.AddRedisService(new RedisConfiguration() { });
            //var serviceProvider = services.BuildServiceProvider();
            //Assert.ThrowsAny<Exception>(() => { serviceProvider.GetService<IRedisService>(); });
        }


        [Theory]
        [InlineData(null)]
        public void TestCreateNull(RedisConfiguration config)
        {
            var services = new ServiceCollection();
            Assert.ThrowsAny<Exception>(() => { services.AddRedisService(config); });
        }

        #region add

        public class NullParaData : TheoryData<string?, string?>
        {
            public NullParaData()
            {
                Add("", "x");
                Add(null, "x");
            }
        }

        [Theory]
        [ClassData(typeof(NullParaData))]
        public async Task AddNullParaTest(string key, string value)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.AddAsync(key, value));
        }


        public class NullParaWithDatetimeData : TheoryData<string?, string?, TimeSpan?>
        {
            public NullParaWithDatetimeData()
            {
                Add("", "x", TimeSpan.FromSeconds(3));
                Add(null, "x", TimeSpan.FromSeconds(3));
                Add("", "x", null);
                Add(null, "x", null);
            }
        }
        [Theory]
        [ClassData(typeof(NullParaWithDatetimeData))]
        public async Task AddNullParaWithDatetimeTest(string key, string value, TimeSpan? expiry)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.AddAsync(key, value, expiry));
        }


        public class AddExpectData : TheoryData<string, object?>
        {
            public AddExpectData()
            {
                Add("k1", "v1");
                Add("k2", "1");
                Add("k3", "2.00331");
                Add("k4", 2);
                Add("k5", 1.123131M);
                Add("k6", null);
                Add("k7", "");
            }
        }

        [Theory]
        [ClassData(typeof(AddExpectData))]
        public async Task AddExpectTest(string key, object? value)
        {
            Assert.True(await _redis.AddAsync(key, value));
            Assert.Equal(value?.ToString(), await _redis.GetAsync(key));
        }

        public class AddStringExpectData : TheoryData<string, string?>
        {
            public AddStringExpectData()
            {
                Add("k1", "v1");
                Add("k2", "1");
                Add("k3", "2.00331");
                Add("k6", null);
                Add("k7", "");
            }
        }

        [Theory]
        [ClassData(typeof(AddStringExpectData))]
        public async Task AddStringExpectTest(string key, string? value)
        {
            Assert.True(await _redis.AddAsync(key, value));
            Assert.Equal(value?.ToString(), await _redis.GetAsync(key));
        }

        public class AddIntExpectData : TheoryData<string, int?>
        {
            public AddIntExpectData()
            {
                Add("k4", 2);
                Add("k5", 0);
                Add("k5", -1);
                Add("k6", null);
            }
        }

        [Theory]
        [ClassData(typeof(AddIntExpectData))]
        public async Task AddIntExpectTest(string key, int? value)
        {
            Assert.True(await _redis.AddAsync(key, value));
            Assert.Equal(value?.ToString(), await _redis.GetAsync(key));
        }



        public class AddExpectWithSecondsData : TheoryData<string, object, int>
        {
            public AddExpectWithSecondsData()
            {
                Add("k1", "v1", 2);
                Add("k2", "1", 3);
            }
        }

        [Theory]
        [ClassData(typeof(AddExpectWithSecondsData))]
        public async Task AddExpectWithSecondsTest(string key, object value, int expiry)
        {
            Assert.True(await _redis.AddAsync(key, value, TimeSpan.FromSeconds(expiry)));
            Assert.Equal(value.ToString(), await _redis.GetAsync(key));
            await Task.Delay((expiry + 1) * 1000);
            Assert.Null(await _redis.GetAsync(key));
        }


        public class CacheTestData : TheoryData<string, TestModel?>
        {
            public CacheTestData()
            {
                Add("k1", new TestModel() { Id = 1, Name = "name1" });
                Add("k2", new TestModel() { Id = 2, Name = "name2" });
                Add("k3", new TestModel() { Id = 3, Name = "" });
                Add("k4", null);
            }
        }

        [Theory]
        [ClassData(typeof(CacheTestData))]
        public async Task AddClassExpectTest(string key, TestModel value)
        {
            Assert.True(await _redis.AddAsync(key, value));
            Assert.Equal(JsonConvert.SerializeObject(value), JsonConvert.SerializeObject(await _redis.GetAsync<TestModel>(key)));
        }
        #endregion

        #region remove
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RemoveNullTest(string key)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.RemoveAsync(key));
        }

        [Theory]
        [InlineData("k1", "v1")]
        [InlineData("k2", "v2")]
        [InlineData("k3", "v3")]
        [InlineData("k4", "v4")]
        public async Task RemoveTest(string key, string value)
        {
            await _redis.AddAsync(key, value);
            Assert.NotNull(await _redis.GetAsync(key));
            await _redis.RemoveAsync(key);
            Assert.Null(await _redis.GetAsync(key));
        }

        #endregion

        #region get

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GeExceptionTest(string key)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.GetAsync<TestModel>(key));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.GetAsync(key));
        }
        [Theory]
        [InlineData("k1", "")]
        [InlineData("k1", null)]
        public async Task GetNullTest(string key, string value)
        {
            await _redis.AddAsync(key, value);
            var v1 = await _redis.GetAsync<TestModel>(key);
            Assert.Null(v1);
            var v2 = await _redis.GetAsync(key);
            Assert.Equal(value, v2);
            Assert.True(string.IsNullOrEmpty(v2));
            await _redis.RemoveAsync(key);
        }


        public class GetStringData : TheoryData<string, string>
        {
            public GetStringData()
            {
                Add("k1", "-13213");
                Add("k2", "0");
                Add("k3", "6789");
                Add("k4", "逗我啊打大我打完打啊打");
                Add("k5", "gfwafkadawfafa");
                Add("k5", "~!@#$%^&*()_+<>?{}");
            }
        }

        [Theory]
        [ClassData(typeof(GetStringData))]
        public async Task GetStringTest(string key, string value)
        {
            await _redis.AddAsync(key, value);
            Assert.Equal(value, await _redis.GetAsync(key));
            await _redis.RemoveAsync(key);
        }
        #endregion

        #region lock
        [Fact]
        public async Task LockAsync_ValidKey_ReturnsTrue()
        {
            // Arrange
            var key = "myKey";
            var expiry = TimeSpan.FromMinutes(5);

            // Act
            var result = await _redis.LockAsync(key, expiry);

            // Assert
            Assert.True(result);
        }
        public class NullKeyData : TheoryData<string?>
        {
            public NullKeyData()
            {
                Add("");
                Add(null);
            }
        }

        [Theory]
        [ClassData(typeof(NullKeyData))]
        public async Task LockAsync_NullKey_ThrowsArgumentNullException(string key)
        {
            var expiry = TimeSpan.FromMinutes(5);
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.LockAsync(key, expiry));
        }

        [Fact]
        public async Task UnLockAsync_ValidKey_ReturnsTrue()
        {
            // Arrange
            var key = "myKey"; 

            // Act
            var result = await _redis.UnLockAsync(key);

            // Assert
            Assert.True(result);
        }        

        [Theory]
        [ClassData(typeof(NullKeyData))]
        public async Task UnLockAsync_NullKey_ThrowsArgumentNullException(string key)
        { 
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _redis.UnLockAsync(key));
        }
        #endregion
    }
}
