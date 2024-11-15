using Bogus;
using jfYu.Core.Redis.Interface;
using jfYu.Core.Test.Models;
using StackExchange.Redis;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisStringTests(IRedisService redisService)
    {
        private readonly IRedisService _redisService = redisService;

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ExistsAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ExistsAsync(key));
            if (key == null)
                Assert.IsType<ArgumentNullException>(ex);
            else
                Assert.IsType<ArgumentException>(ex);
             
        }

        [Fact]
        public async Task ExistsAsync_KeyDoesNotExist_ReturnsFalse()
        {
            var result = await _redisService.ExistsAsync("nonexistent_key");
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_KeyExists_ReturnsTrue()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.ExistsAsync("existent_key");
            Assert.True(result);
            await _redisService.RemoveAsync("existent_key");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task RemoveAsync_KeyIsNull_ThrowsException(string key)
        { 
            var ex = await Record.ExceptionAsync(() => _redisService.RemoveAsync(key));
            if (key == null)
                Assert.IsType<ArgumentNullException>(ex);
            else
                Assert.IsType<ArgumentException>(ex);
        }


        [Fact]
        public async Task RemoveAsync_KeyDoesNotExist_ReturnsFalse()
        {
            var result = await _redisService.RemoveAsync("nonexistent_key");
            Assert.False(result);
        }

        [Fact]
        public async Task RemoveAsync_KeyExists_ReturnsTrue()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.RemoveAsync("existent_key");
            Assert.True(result);
        }

        public class NullKeysExpectData : TheoryData<List<string>>
        {
            public NullKeysExpectData()
            {
                Add(default);
                Add([]);
            }
        }

        [Theory]
        [ClassData(typeof(NullKeysExpectData))]
        public async Task RemoveAllAsync_KeyIsNull_ThrowsException(List<string> keys)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _redisService.RemoveAllAsync(keys));
        }


        [Fact]
        public async Task RemoveAllAsync_AllKeysExist_ReturnsNumberOfKeysRemoved()
        {
            await _redisService.AddAsync("{user:123}:key1", "value1");
            await _redisService.AddAsync("{user:123}:key2", "value2");
            await _redisService.AddAsync("{user:123}:key3", "value3");

            var result = await _redisService.RemoveAllAsync(["{user:123}:key1", "{user:123}:key2", "{user:123}:key3"]);
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task RemoveAllAsync_AllKeysNotExist_ReturnsNumberOfKeysRemoved()
        {

            var result = await _redisService.RemoveAllAsync(["{user:123}:key1", "{user:123}:key2", "{user:123}:key3"]);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task RemoveAllAsync_AllKeysHalfExist_ReturnsNumberOfKeysRemoved()
        {
            await _redisService.AddAsync("{user:123}:key1", "value1");
            await _redisService.AddAsync("{user:123}:key2", "value2");

            var result = await _redisService.RemoveAllAsync(["{user:123}:key1", "{user:123}:key2", "{user:123}:key3"]);
            Assert.Equal(2, result);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.GetAsync<string>(key));
            if (key == null)
                Assert.IsType<ArgumentNullException>(ex);
            else
                Assert.IsType<ArgumentException>(ex);
             
        }


        [Fact]
        public async Task GetAsync_KeyDoesNotExist_ReturnsNull()
        {
            var result = await _redisService.GetAsync<string>("nonexistent_key");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_KeyExists_ReturnsValue()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.GetAsync<string>("existent_key");
            Assert.Equal("value", result);
            await _redisService.RemoveAsync("existent_key");
        }

        [Fact]
        public async Task GetAsync_ValueInt_ReturnsValue()
        {
            var key = "key1";
            var value = 1;
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<int>(key);
            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task GetAsync_ValueLong_ReturnsValue()
        {
            var key = "key1";
            long value = 121132321321;
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<long>(key);
            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task GetAsync_ValueDouble_ReturnsValue()
        {
            var key = "key1";
            double value = 121132321321.3123131414;
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<double>(key);
            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task GetAsync_ValueDecimal_ReturnsValue()
        {
            var key = "key1";
            decimal value = 12113232132131.3114142131M;
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<decimal>(key);
            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task GetAsync_ValueDateTime_ReturnsValue()
        {
            var key = "key1";
            DateTime value = DateTime.UtcNow;
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<DateTime>(key);
            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task GetAsync_ValueModel_ReturnsValue()
        {
            var key = "key1";
            var value = new ModelFaker().GenerateBetween(1, 10);
            await _redisService.AddAsync(key, value);
            var result = await _redisService.GetAsync<List<TestModel>>(key);
            Assert.Equal(value, result, new TestModelComparer());
            await _redisService.RemoveAsync(key);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetAsync_WithExpiry_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.GetAsync<string>(key, TimeSpan.FromSeconds(10)));
            if (key == null)
                Assert.IsType<ArgumentNullException>(ex);
            else
                Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public async Task GetAsync_WithExpiry_KeyDoesNotExist_ReturnsNull()
        {
            var result = await _redisService.GetAsync<string>("nonexistent_key", TimeSpan.FromSeconds(10));
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAsync_WithExpiry_KeyExist_ReturnsNull()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.GetAsync<string>("existent_key", TimeSpan.FromSeconds(3));
            Assert.Equal("value", result);
            await Task.Delay(5000);
            result = await _redisService.GetAsync<string>("existent_key");
            Assert.Null(result);
            await _redisService.RemoveAsync("existent_key");
        }


        [Fact]
        public async Task GetAsync_WithExpiry_KeyExist_ReturnsValue()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.GetAsync<string>("existent_key", TimeSpan.FromSeconds(5));
            Assert.Equal("value", result);
            await Task.Delay(3000);
            result = await _redisService.GetAsync<string>("existent_key");
            Assert.Equal("value", result);
            await Task.Delay(3000);
            result = await _redisService.GetAsync<string>("existent_key");
            Assert.Null(result);
            await _redisService.RemoveAsync("existent_key");
        }
        [Fact]
        public async Task GetAsync_WithExpiry_KeyExists_ReturnsValueAndUpdatesExpiry()
        {
            var expiry = TimeSpan.FromSeconds(10);
            await _redisService.AddAsync("key_with_expiry", "value", expiry);
            var result = await _redisService.GetAsync<string>("key_with_expiry", expiry);
            Assert.Equal("value", result);
            await _redisService.RemoveAsync("key_with_expiry");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task AddAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex =await Record.ExceptionAsync(() => _redisService.AddAsync(key, ""));
            if (key == null)
                Assert.IsType<ArgumentNullException>(ex);
            else
                Assert.IsType<ArgumentException>(ex);
        }


        [Fact]
        public async Task AddAsync_KeyAlreadyExists_ReturnsNewValue()
        {
            await _redisService.AddAsync("existent_key", "value");
            var result = await _redisService.AddAsync("existent_key", "new_value");
            Assert.True(result);
            var value = await _redisService.GetAsync<string>("existent_key");
            Assert.Equal("new_value", value);
            await _redisService.RemoveAsync("existent_key");
        }

        [Fact]
        public async Task AddAsync_KeyDoesNotExist_ReturnsTrue()
        {
            var result = await _redisService.AddAsync("nonexistent_key", "value");
            Assert.True(result);
            await _redisService.RemoveAsync("nonexistent_key");
        }



        [Fact]
        public async Task AddAsync_WithExpiry_KeyDoesNotExist_ReturnsTrue()
        {
            var expiry = TimeSpan.FromSeconds(3);
            var result = await _redisService.AddAsync("key_with_expiry", "value", expiry);
            Assert.True(result);
            await Task.Delay(5000);
            var value = await _redisService.GetAsync<string>("key_with_expiry");
            Assert.Null(value);
            await _redisService.RemoveAsync("key_with_expiry");
        }

        [Fact]
        public async Task AddAsync_WithExpiry_KeyExist_ReturnsNewValue()
        {
            await _redisService.AddAsync("key_with_expiry", "value");
            var expiry = TimeSpan.FromSeconds(10);
            var result = await _redisService.AddAsync("key_with_expiry", "new_value", expiry);
            Assert.True(result);
            var value = await _redisService.GetAsync<string>("key_with_expiry");
            Assert.Equal("new_value", value);
            await _redisService.RemoveAsync("key_with_expiry");
        }


        [Fact]
        public async Task AddAsync_WithCondition_KeyDoesNotExist_ReturnsTrue()
        {
            var result = await _redisService.AddAsync("conditional_key", "value", null, When.NotExists);
            Assert.True(result);
            await _redisService.RemoveAsync("conditional_key");
        }

        [Fact]
        public async Task AddAsync_WithCondition_KeyExists_ReturnsFalse()
        {
            await _redisService.AddAsync("conditional_key", "value");
            var result = await _redisService.AddAsync("conditional_key", "new_value", null, When.NotExists);
            Assert.False(result);
            await _redisService.RemoveAsync("conditional_key");
        }
    }
}
