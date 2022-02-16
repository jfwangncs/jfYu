using System;
using System.Collections.Generic;
using System.Text;

namespace jfYu.Core.MongoDB
{
    public class MongoDBConfig
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string MongoUrl { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 最大连接池
        /// </summary>
        public int MaxConnectionPoolSize { get; set; } = 500;

        /// <summary>
        /// 最大闲置时间
        /// </summary>
        public int MaxConnectionIdleTime { get; set; } = 30;

        /// <summary>
        /// 最大存活时间
        /// </summary>
        public int MaxConnectionLifeTime { get; set; } = 60;

        /// <summary>
        /// 链接时间
        /// </summary>
        public int ConnectTimeout { get; set; } = 30;

        /// <summary>
        /// 等待队列大小
        /// </summary>
        public int WaitQueueSize { get; set; } = 100;

        /// <summary>
        /// socket超时时间
        /// </summary>
        public int SocketTimeout { get; set; } = 30;

        /// <summary>
        /// 队列等待时间
        /// </summary>
        public int WaitQueueTimeout { get; set; } = 60;

    }
}
