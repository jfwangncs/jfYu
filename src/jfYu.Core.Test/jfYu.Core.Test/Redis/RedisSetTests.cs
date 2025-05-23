﻿using jfYu.Core.Redis.Interface;
using jfYu.Core.Test.Models;
using static jfYu.Core.Test.Redis.RedisBaseTests;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class RedisSetTests(IRedisService redisService)
    {
        private readonly IRedisService _redisService = redisService;

        #region SetAdd

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task SetAddAsync_WhenKeyOrValueIsNull_ShouldThrowArgumentException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetAddAsync(key, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetAddAsync_ValueNotExist_ReturnTrue()
        {
            string key = "testKey";
            var value = "v1";
            bool result = await _redisService.SetAddAsync(key, value);
            Assert.True(result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetAddAsync_ValueExist_ReturnFalse()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            bool result = await _redisService.SetAddAsync(key, value);
            Assert.False(result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetAdd

        #region SetAddAll

        [Theory]
        [ClassData(typeof(NullKeyAndValuesExpectData))]
        public async Task SetAddAllAsync_WhenKeyOrValueIsNull_ShouldThrowArgumentException(string key, List<string> values)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetAddAllAsync(key, values));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetAddAllAsync_ValuesNotExist_ReturnCorrectLength()
        {
            string key = "testKey";
            List<string> values = ["v1", "v2", "v3"];
            var result = await _redisService.SetAddAllAsync(key, values);
            Assert.Equal(values.Count, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetAddAllAsync_ValuesExistPartially_ReturnCorrectLength()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            List<string> values = ["v1", "v2", "v3"];
            var result = await _redisService.SetAddAllAsync(key, values);
            Assert.Equal(2, result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetAddAll

        #region SetRemoveAsync

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task SetRemoveAsync_WhenKeyOrValueIsNull_ShouldThrowArgumentException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetRemoveAsync(key, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetRemoveAsync_ValuesNotExist_ReturnFalse()
        {
            string key = "testKey";
            var value = "v1";
            var result = await _redisService.SetRemoveAsync(key, value);
            Assert.False(result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetRemoveAsync_ValuesExistPartially_ReturnTrue()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            var result = await _redisService.SetRemoveAsync(key, value);
            Assert.True(result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetRemoveAsync

        #region SetRemoveAllAsync

        [Theory]
        [ClassData(typeof(NullKeyAndValuesExpectData))]
        public async Task SetRemoveAllAsync_WhenKeyOrValueIsNull_ShouldThrowArgumentException(string key, List<string> values)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetRemoveAllAsync(key, values));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetRemoveAllAsync_ValuesNotExist_ReturnCorrectLength()
        {
            string key = "testKey";
            List<string> values = ["v1", "v2", "v3"];
            var result = await _redisService.SetRemoveAllAsync(key, values);
            Assert.Equal(0, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetRemoveAllAsync_ValuesExist_ReturnCorrectLength()
        {
            string key = "testKey";
            List<string> values = ["v1", "v2", "v3"];
            await _redisService.SetAddAllAsync(key, values);
            var result = await _redisService.SetRemoveAllAsync(key, values);
            Assert.Equal(values.Count, result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetRemoveAllAsync_ValuesExistPartially_ReturnCorrectLength()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            List<string> values = ["v1", "v2", "v3"];
            var result = await _redisService.SetRemoveAllAsync(key, values);
            Assert.Equal(1, result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetRemoveAllAsync

        #region SetContainsAsync

        [Theory]
        [ClassData(typeof(NullKeyAndValueExpectData))]
        public async Task SetContainsAsync_WhenKeyOrValueIsNull_ShouldThrowArgumentException(string key, string value)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetContainsAsync(key, value));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetContainsAsync_KeyNotExist_ReturnFalse()
        {
            string key = "testKey";
            var value = "v1";
            bool result = await _redisService.SetContainsAsync(key, value);
            Assert.False(result); ;
        }

        [Fact]
        public async Task SetContainsAsync_ValueNotExist_ReturnFalse()
        {
            string key = "testKey";
            var value = "v1";
            bool result = await _redisService.SetContainsAsync(key, value);
            Assert.False(result);
        }

        [Fact]
        public async Task SetContainsAsync_ValueExist_ReturnTrue()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            var result = await _redisService.SetContainsAsync(key, value);
            Assert.True(result);
            await _redisService.RemoveAsync(key);
        }

        [Fact]
        public async Task SetContainsAsync_complexValueExist_ReturnTrue()
        {
            string key = "testKey";
            var value = new TestModelFaker().Generate();
            await _redisService.SetAddAsync(key, value);
            var result = await _redisService.SetContainsAsync(key, value);
            Assert.True(result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetContainsAsync

        #region SetMembersAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task SetMembersAsync_WhenKeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetMembersAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetMembersAsync_KeyNotExist_ReturnCorrectly()
        {
            string key = "testKey";
            var result = await _redisService.SetMembersAsync(key);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SetMembersAsync_ValueNotExist_ReturnCorrectly()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            await _redisService.SetRemoveAsync(key, value);
            var result = await _redisService.SetMembersAsync(key);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SetMembersAsync_ReturnCorrectly()
        {
            string key = "testKey";
            var value = new TestModelFaker().Generate();
            List<object> values = ["v1", 1, "v3", 423.442, value, new TestModelFaker().Generate(2), "", new TestModelSubFaker().Generate()];
            await _redisService.SetAddAllAsync(key, values);
            var result = await _redisService.SetMembersAsync(key);
            Assert.Equal(values.Count, result.Count);
            await _redisService.SetRemoveAsync(key, value);
            result = await _redisService.SetMembersAsync(key);
            Assert.Equal(values.Count - 1, result.Count);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetMembersAsync

        #region SetLengthAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task SetLengthAsync_WhenKeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetLengthAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetLengthAsync_KeyNotExist_ReturnCorrectly()
        {
            string key = "testKey";
            var result = await _redisService.SetLengthAsync(key);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task SetLengthAsync_ValueNotExist_ReturnCorrectly()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            await _redisService.SetRemoveAsync(key, value);
            var result = await _redisService.SetLengthAsync(key);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task SetLengthAsync_ReturnCorrectly()
        {
            string key = "testKey";
            List<object> values = ["v1", 1, "v3", 423.442, new TestModelFaker().Generate(), new TestModelFaker().Generate(2), "", new TestModelSubFaker().Generate()];
            await _redisService.SetAddAllAsync(key, values);
            var result = await _redisService.SetLengthAsync(key);
            Assert.Equal(values.Count, result);
            await _redisService.RemoveAsync(key);
        }

        #endregion SetLengthAsync

        #region SetRandomMemberAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task SetRandomMemberAsync_WhenKeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetRandomMemberAsync(key));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetRandomMemberAsync_KeyNotExist_ReturnEmpty()
        {
            string key = "testKey";
            var result = await _redisService.SetRandomMemberAsync(key);
            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task SetRandomMemberAsync_ValueNotExist_ReturnEmpty()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            await _redisService.SetRemoveAsync(key, value);
            var result = await _redisService.SetRandomMemberAsync(key);
            Assert.False(result.HasValue);
        }

        [Fact]
        public async Task SetRandomMemberAsync_WithOneStringValue_ReturnOne()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            var result = await _redisService.SetRandomMemberAsync(key);
            Assert.True(result.HasValue);
            Assert.Equal(value, _redisService.Serializer.Deserialize<string>(result!));
        }

        [Fact]
        public async Task SetRandomMemberAsync_WithOneModelValue_ReturnOne()
        {
            string key = "testKey";
            var value = new TestModelFaker().Generate(10);
            await _redisService.SetAddAsync(key, value);
            var result = await _redisService.SetRandomMemberAsync(key);
            Assert.True(result.HasValue);
            Assert.Equal(value, _redisService.Serializer.Deserialize<List<TestModel>>(result!));
        }

        [Fact]
        public async Task SetRandomMemberAsync_WithMoreValues_ReturnOne()
        {
            string key = "testKey";
            List<string> values = ["v1", "v2", "v3", "v4"];
            await _redisService.SetAddAllAsync(key, values);
            var result = await _redisService.SetRandomMemberAsync(key);
            Assert.True(result.HasValue);
            Assert.Contains(_redisService.Serializer.Deserialize<string>(result!), values);
        }

        #endregion SetRandomMemberAsync

        #region SetRandomMembersAsync

        [Theory]
        [ClassData(typeof(NullKeyExpectData))]
        public async Task SetRandomMembersAsync_WhenKeyIsNull_ShouldThrowArgumentException(string key)
        {
            var ex = await Record.ExceptionAsync(async () => await _redisService.SetRandomMembersAsync(key, 1));
            Assert.IsAssignableFrom<ArgumentException>(ex);
        }

        [Fact]
        public async Task SetRandomMembersAsync_KeyNotExist_ReturnEmpty()
        {
            string key = "testKey";
            var result = await _redisService.SetRandomMembersAsync(key, 1);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SetRandomMembersAsync_ValueNotExist_ReturnEmpty()
        {
            string key = "testKey";
            var value = "v1";
            await _redisService.SetAddAsync(key, value);
            await _redisService.SetRemoveAsync(key, value);
            var result = await _redisService.SetRandomMembersAsync(key, 1);
            Assert.Empty(result);
        }

        [Fact]
        public async Task SetRandomMembersAsync_ReturnCorrectly()
        {
            string key = "testKey";
            List<string> values = ["v1", "v2", "v3", "v4"];
            await _redisService.SetAddAllAsync(key, values);
            var result = await _redisService.SetRandomMembersAsync(key, 3);
            Assert.Equal(3, result.Count);
        }

        #endregion SetRandomMembersAsync
    }
}