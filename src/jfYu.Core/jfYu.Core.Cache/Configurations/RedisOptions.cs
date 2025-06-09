using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace jfYu.Core.Cache.Configurations
{
    public class RedisOptions : RedisCacheOptions
    {

        /// <summary>
        /// Gets or sets a value indicating whether enable logging.
        /// </summary>
        /// <value><c>true</c> if enable logging; otherwise, <c>false</c>.</value>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets the expire ms.
        /// default is 0 , means never expire
        /// </summary>
        /// <value>The expire ms.</value>
        public int DefaultExpirationMs { get; set; } = 0;
    }
}
