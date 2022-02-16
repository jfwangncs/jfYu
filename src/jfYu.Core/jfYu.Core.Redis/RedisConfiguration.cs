using System.Collections.Generic;

namespace jfYu.Core.Redis
{
    public class RedisConfiguration
    {

        public List<RedisEndPoint> EndPoints { get; set; }
        /// <summary>
        /// 密码
        /// </summary>

        public string Password { get; set; }

        /// <summary>
        /// 数据库index
        /// </summary>

        public int DbIndex { get; set; } = 0;

        /// <summary>
        /// 超时时间（毫秒）
        /// </summary>

        public int Timeout { get; set; } = 5000;


    }

    public class RedisEndPoint
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
    }
}
