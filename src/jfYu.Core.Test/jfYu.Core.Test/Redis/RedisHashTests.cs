using jfYu.Core.Redis.Interface;
using jfYu.Core.Test.Models;
using StackExchange.Redis;
using static jfYu.Core.Test.Redis.RedisBaseTests;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisHashTests(IRedisService redisService)
    {
        private readonly IRedisService _redisService = redisService;

        #region HashSetAsync
        [Theory]
        [ClassData(typeof(NullkeyAndKeyAndValueExpectData))]
        public async Task HashSetAsync_WhenkeyOrHashKeyIsNull_ShouldThrowArgumentException(string key, string hashKey, string value)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.HashSetAsync(key, hashKey, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task HashSetAsync_SetsValue_ReturnsTrue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            string value = "testValue";
            bool result = await _redisService.HashSetAsync(key, hashKey, value);
            Assert.True(result);
            await _redisService.RemoveAsync("testHash");
        }

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task HashGetAsync_WhenkeyOrKeyIsNull_ShouldThrowArgumentException(string key, string hashKey)
        {

            var ex = await Record.ExceptionAsync(async () => await _redisService.HashGetAsync<int>(key, hashKey));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }
        #endregion

        #region HashGet
        [Fact]
        public async Task HashGetAsync_KetNotExist_ReturnsExpectedValue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            var actualValue = await _redisService.HashGetAsync<string>(key, hashKey);
            Assert.Null(actualValue);
        }

        [Fact]
        public async Task HashGetAsync_RetrievesStringValueWithAddTwice_ReturnsExpectedValue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            string expectedValue = "testValue";
            string expectedValue1 = "testValue";
            await _redisService.HashSetAsync(key, hashKey, expectedValue);
            await _redisService.HashSetAsync(key, hashKey, expectedValue1);
            string? actualValue = await _redisService.HashGetAsync<string>(key, hashKey);
            Assert.Equal(expectedValue1, actualValue);
            await _redisService.RemoveAsync("testHash");
        }

        [Fact]
        public async Task HashGetAsync_RetrievesStringValue_ReturnsExpectedValue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            string expectedValue = "testValue";
            await _redisService.HashSetAsync(key, hashKey, expectedValue);
            string? actualValue = await _redisService.HashGetAsync<string>(key, hashKey);
            Assert.Equal(expectedValue, actualValue);
            await _redisService.RemoveAsync("testHash");
        }

        [Fact]
        public async Task HashGetAsync_RetrievesIntValue_ReturnsExpectedValue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            var expectedValue = 9898;
            await _redisService.HashSetAsync(key, hashKey, expectedValue);
            var actualValue = await _redisService.HashGetAsync<int>(key, hashKey);
            Assert.Equal(expectedValue, actualValue);
            await _redisService.RemoveAsync("testHash");
        }

        [Fact]
        public async Task HashGetAsync_RetrievesModelValue_ReturnsExpectedValue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            var expectedValue = new TestModelFaker().Generate();
            await _redisService.HashSetAsync(key, hashKey, expectedValue);
            var actualValue = await _redisService.HashGetAsync<TestModel>(key, hashKey);
            Assert.Equal(expectedValue, actualValue);
            await _redisService.RemoveAsync("testHash");
        }
        #endregion

        #region HaskGetAll
        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task HashGetAllAsync_WhenkeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.HashGetAllAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }
        [Fact]
        public async Task HashGetAllAsync_RetrievesAllEntries_ReturnsEntries()
        {
            string key = "testHash";
            await _redisService.HashSetAsync(key, "key1", "value1");
            await _redisService.HashSetAsync(key, "key2", "value2");
            HashEntry[] entries = await _redisService.HashGetAllAsync(key);
            Assert.NotNull(entries);
            Assert.Contains(entries, e => e.Name == "key1" && e.Value == _redisService.Serializer.Serialize("value1"));
            Assert.Contains(entries, e => e.Name == "key2" && e.Value == _redisService.Serializer.Serialize("value2"));
            await _redisService.RemoveAsync("testHash");
        }
        #endregion

        #region HashDelete
        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task HashDeleteAsyncHashSetAsync_WhenkeyOrKeyIsNull_ShouldThrowArgumentException(string key, string hashKey)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.HashDeleteAsync(key, hashKey));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task HashDeleteAsync_DeletesValue_ReturnsTrue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            await _redisService.HashSetAsync(key, hashKey, "anyValue");
            bool result = await _redisService.HashDeleteAsync(key, hashKey);
            Assert.True(result);
            await _redisService.RemoveAsync("testHash");
        }

        [Fact]
        public async Task HashDeleteAsync_DeletesValue_ReturnsFalse()
        {
            string key = "testHash";
            string hashKey = "testKey";
            bool result = await _redisService.HashDeleteAsync(key, hashKey);
            Assert.False(result);
        }
        #endregion

        #region HashExists
        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task HashExistsAsyncHashSetAsync_WhenkeyOrKeyIsNull_ShouldThrowArgumentException(string key, string hashKey)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.HashExistsAsync(key, hashKey));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task HashExistsAsync_ChecksValueExistence_ReturnsTrue()
        {
            string key = "testHash";
            string hashKey = "testKey";
            await _redisService.HashSetAsync(key, hashKey, "anyValue");
            bool exists = await _redisService.HashExistsAsync(key, hashKey);
            Assert.True(exists);
            await _redisService.RemoveAsync("testHash");
        }
        [Fact]
        public async Task HashExistsAsync_ChecksValueExistence_ReturnsFalse()
        {
            string key = "testHash";
            string hashKey = "testKey";
            bool exists = await _redisService.HashExistsAsync(key, hashKey);
            Assert.False(exists);
        } 
        #endregion

    }
}
