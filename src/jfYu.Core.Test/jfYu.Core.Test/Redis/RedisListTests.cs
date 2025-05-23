﻿using jfYu.Core.Redis.Interface;
using jfYu.Core.Test.Models;
using static jfYu.Core.Test.Redis.RedisBaseTests;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisListTestss(IRedisService redisService)
    {
        private readonly IRedisService _redisService = redisService;

        #region ListAddAsync

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task ListAddAsync_KeyIsNull_ThrowsException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListAddAsync(key, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListAddAsync_ValidInput_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            var result = await _redisService.ListAddAsync(key, value);

            Assert.True(result > 0);
            await _redisService.RemoveAsync(key);
        }

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task ListAddToLeftAsync_KeyIsNull_ThrowsException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListAddToLeftAsync(key, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListAddToLeftAsync_ValidInput_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            var result = await _redisService.ListAddToLeftAsync(key, value);

            Assert.True(result > 0);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListAddAsync

        #region ListPopFromRightAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task ListPopFromRightAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListPopFromRightAsync<string>(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListPopFromRightAsync_KeyNotExist_ReturnsNull()
        {
            var key = "testKey";
            await _redisService.ListPopFromRightAsync<string>(key);
            Assert.Null(null);
        }

        [Fact]
        public async Task ListPopFromRightAsync_ValidInput_ReturnsValue()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value);
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            var result = await _redisService.ListPopFromRightAsync<string>(key);

            Assert.Equal(value + "2", result);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListPopFromRightAsync

        #region ListPopFromLeftAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task ListPopFromLeftAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListPopFromLeftAsync<string>(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListPopFromLeftAsync_KeyNotExist_ReturnsNull()
        {
            var key = "testKey";
            await _redisService.ListPopFromLeftAsync<string>(key);
            Assert.Null(null);
        }

        [Fact]
        public async Task ListPopFromLeftAsync_ValidInput_ReturnsValue()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value);
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            var result = await _redisService.ListPopFromLeftAsync<string>(key);

            Assert.Equal(value, result);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListPopFromLeftAsync

        #region ListLengthAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task ListLengthAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListLengthAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListLengthAsync_KeyNotExist_ReturnsNull()
        {
            var key = "testKey";
            await _redisService.ListLengthAsync(key);
            Assert.Null(null);
        }

        [Fact]
        public async Task ListLengthAsync_ValidInput_ReturnsLong()
        {
            var key = "testKey";

            await _redisService.ListAddAsync(key, "testValue");
            await _redisService.ListAddAsync(key, "testValue");
            await _redisService.ListAddAsync(key, "testValue");
            await _redisService.ListAddAsync(key, "testValue");
            await _redisService.ListAddAsync(key, "testValue");
            var result = await _redisService.ListLengthAsync(key);

            Assert.Equal(5, result);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListLengthAsync

        #region ListRemoveAsync

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task ListRemoveAsync_KeyIsNull_ThrowsException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListRemoveAsync(key, value, 0));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListRemoveAsync_KeyNotExist_ReturnsNull()
        {
            var key = "testKey";
            await _redisService.ListRemoveAsync(key, "value", 0);
            Assert.Null(null);
        }

        [Fact]
        public async Task ListRemoveAsync_CountEqual0_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "4");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListRemoveAsync(key, value + "0", 0);
            Assert.Equal(3, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListRemoveAsync_CountGreaterThan0_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "4");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "8");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListRemoveAsync(key, value + "0", 2);
            Assert.Equal(2, result);
            var pop = await _redisService.ListPopFromLeftAsync<string>(key);
            Assert.Equal(value + "1", pop);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListRemoveAsync_CountGreaterThanActual_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "4");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListRemoveAsync(key, value + "0", 5);
            Assert.Equal(3, result);
            var pop = await _redisService.ListPopFromLeftAsync<string>(key);
            Assert.Equal(value + "1", pop);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListRemoveAsync_CountLessThan0_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "4");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "8");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListRemoveAsync(key, value + "0", -1);
            Assert.Equal(1, result);
            var pop = await _redisService.ListPopFromRightAsync<string>(key);
            Assert.Equal(value + "8", pop);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListRemoveAsync_CountLessThanActual_ReturnsLong()
        {
            var key = "testKey";
            var value = "testValue";

            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "4");
            await _redisService.ListAddAsync(key, value + "3");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, value + "8");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListRemoveAsync(key, value + "0", -5);
            Assert.Equal(3, result);
            var pop = await _redisService.ListPopFromRightAsync<string>(key);
            Assert.Equal(value + "8", pop);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListRemoveAsync

        #region ListGetRangeAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task ListGetRangeAsync_KeyIsNull_ThrowsException(string key)
        {
            var ex = await Record.ExceptionAsync(() => _redisService.ListGetRangeAsync(key, 0, 0));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task ListGetRangeAsync_KeyNotExist_ReturnsNull()
        {
            var key = "testKey";
            await _redisService.ListGetRangeAsync(key, -1110, -1);
            Assert.Null(null);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(0, -11)]
        [InlineData(11, 13)]
        public async Task ListGetRangeAsync_InValidInput_ReturnsList(int start, int stop)
        {
            var key = "testKey";
            await _redisService.ListAddAsync(key, "value");
            var result = await _redisService.ListGetRangeAsync(key, start, stop);
            Assert.Empty(result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListGetRangeAsync_ValidInput_ReturnsList()
        {
            var key = "testKey";
            var value = "testValue";

            var v1 = new TestModelFaker().Generate();
            var v2 = new TestModelSubFaker().Generate();
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, v1);
            await _redisService.ListAddAsync(key, v2);
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListGetRangeAsync(key, 3, 4);
            var r1 = result[0];
            var r2 = result[1];
            Assert.True(r1.HasValue);
            Assert.True(r2.HasValue);

            Assert.Equal(v1, _redisService.Serializer.Deserialize<TestModel>(r1!));
            Assert.Equal(v2, _redisService.Serializer.Deserialize<TestSubModel>(r2!));
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task ListGetRangeAsync_ReturnsAll()
        {
            var key = "testKey";
            var value = "testValue";

            var v1 = new TestModelFaker().Generate();
            var v2 = new TestModelSubFaker().Generate();
            await _redisService.ListAddAsync(key, value + "0");
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "2");
            await _redisService.ListAddAsync(key, v1);
            await _redisService.ListAddAsync(key, v2);
            await _redisService.ListAddAsync(key, value + "1");
            await _redisService.ListAddAsync(key, value + "0");
            var result = await _redisService.ListGetRangeAsync(key, 0);
            Assert.True(result.Count == 7);
            await _redisService.RemoveAsync(key);
        }

        #endregion ListGetRangeAsync
    }
}