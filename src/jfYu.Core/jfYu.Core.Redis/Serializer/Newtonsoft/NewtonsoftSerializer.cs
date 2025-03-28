using Newtonsoft.Json;
using System.Text;

namespace jfYu.Core.Redis.Serializer.Newtonsoft
{
    /// <summary>
    /// implementation of <see cref="ISerializer"/>
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NewtonsoftSerializer"/> class.
    /// </remarks>
    /// <param name="settings">The settings.</param>
    public class NewtonsoftSerializer(JsonSerializerSettings settings) : ISerializer
    {
        /// <summary>
        /// Encoding to use to convert string to byte[] and the other way around.
        /// </summary>   
        private static readonly Encoding _encoding = Encoding.UTF8;

        private readonly JsonSerializerSettings _settings = settings;

        /// <inheritdoc/>
        public byte[] Serialize<T>(T? item)
        {
            if (item == null)
                return [];

            var type = item.GetType();
            var jsonString = JsonConvert.SerializeObject(item, type, _settings);
            return _encoding.GetBytes(jsonString);
        }

        /// <inheritdoc/>
        public T? Deserialize<T>(byte[] serializedObject)
        {
            var jsonString = _encoding.GetString(serializedObject);
            return JsonConvert.DeserializeObject<T>(jsonString, _settings)!;
        }
    }
}
