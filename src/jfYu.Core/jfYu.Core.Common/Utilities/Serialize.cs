using Newtonsoft.Json;

namespace jfYu.Core.Common.Utilities
{
    public class Serializer
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="json">json</param>
        /// <returns>数据</returns>
        public static T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }



        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="t">object</param>
        /// <returns>json</returns>
        public static string Serialize(object t)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            return JsonConvert.SerializeObject(t,settings);
        }
    }
}
