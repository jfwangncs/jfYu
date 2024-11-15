using MessagePack;
using MessagePack.Resolvers;
using System;


namespace jfYu.Core.Redis.Serializer
{

    /// <summary>
    /// MsgPac implementation of <see cref="ISerializer"/>
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="MsgPackObjectSerializer"/> class.
    /// </remarks>
    /// <param name="settings">The settings.</param>
    /// <param name="settings">The encoding.</param>
    public class MsgPackObjectSerializer(MessagePackSerializerOptions settings) : ISerializer
    {

        private readonly MessagePackSerializerOptions _settings = settings;

        /// <inheritdoc/>
        public T? Deserialize<T>(byte[] serializedObject)
        {
           return MessagePackSerializer.Deserialize<T>(serializedObject, _settings);
        }

        /// <inheritdoc/>
        public byte[] Serialize<T>(T? item)
        {

            if (item == null)
                return [];

            return MessagePackSerializer.Serialize(item.GetType(), item, _settings);
        }
    }
}
