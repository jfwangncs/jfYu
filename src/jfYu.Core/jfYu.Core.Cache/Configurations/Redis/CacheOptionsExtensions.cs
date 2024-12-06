using System;

namespace jfYu.Core.Cache.Configurations.Redis
{
    /// <summary>
    /// options extensions
    /// </summary>
    public static class CacheOptionsExtensions
    {

        // <summary>
        /// Uses the hybrid (specify the config via hard code).
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="configure">Configure.</param>
        /// <param name="name">The name of this hybrid provider instance.</param>
        public static CacheOptions UseRedis(
            this CacheOptions options
            , Action<RedisOptions> configure
            )
        {
            options.OptionsExtension = new RedisOptionsExtensions(configure);

            return options;
        }

    }
}
