﻿using jfYu.Core.Redis.Serializer.MessagePack;
using jfYu.Core.Test.Models;
using MessagePack;
using MessagePack.Resolvers;

namespace jfYu.Core.Test.Redis
{
    [Collection("Redis")]
    public class MsgPackObjectSerializerTests
    {
        private readonly MsgPackObjectSerializer _serializer;

        public MsgPackObjectSerializerTests()
        {
            var options = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);
            _serializer = new MsgPackObjectSerializer(options);
        }

        [Fact]
        public void Serialize_NonNullObject_ReturnsByteArray()
        {
            var obj = new TestModelFaker().Generate();
            var result = _serializer.Serialize(obj);

            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Serialize_NullObject_ReturnsEmptyByteArray()
        {
            var result = _serializer.Serialize<object>(null);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void Deserialize_ValidByteArray_ReturnsObject()
        {
            var obj = new TestModelFaker().Generate();
            var serialized = _serializer.Serialize(obj);
            var deserialized = _serializer.Deserialize<TestModel>(serialized);

            Assert.NotNull(deserialized);
            Assert.Equal(obj, deserialized);
        }
    }
}