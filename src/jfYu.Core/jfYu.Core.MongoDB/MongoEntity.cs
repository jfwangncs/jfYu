using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace jfYu.Core.MongoDB
{
    public abstract class MongoEntity
    {
        /// <summary>
        /// MongoDB编号
        /// </summary>
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }

        /// <summary>
        /// 0禁用 1正常
        /// </summary>
        [DisplayName("状态")]
        public int State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("修改时间")]
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
