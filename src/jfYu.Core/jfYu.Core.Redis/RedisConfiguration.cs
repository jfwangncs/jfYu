using System.Collections.Generic;

namespace jfYu.Core.Redis
{
    public class RedisConfiguration
    {
        /// <summary>
        /// endpoints
        /// </summary>
        public List<RedisEndPoint> EndPoints { get; set; } = [];

        /// <summary>
        /// password
        /// </summary>

        public string? Password { get; set; }

        /// <summary>
        /// database index default:0
        /// </summary>

        public int DbIndex { get; set; } = 0;

        /// <summary>
        /// timeout default:5000 Milliseconds
        /// </summary>

        public int Timeout { get; set; } = 5000;


        /// <summary>
        /// ssl
        /// </summary>
        public bool Ssl { get; set; } = false;
    }

    public class RedisEndPoint
    {
        /// <summary>
        /// host url
        /// </summary>
        public string Host { get; set; } = "";

        /// <summary>
        /// port
        /// </summary>
        public int Port { get; set; }
    }
}
