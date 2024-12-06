namespace jfYu.Core.Cache.Configurations
{
    /// <summary>
    /// Base options.
    /// </summary>
    public class BaseOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether enable logging.
        /// </summary>
        /// <value><c>true</c> if enable logging; otherwise, <c>false</c>.</value>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets the expire second.
        /// default is 0 means never expire
        /// </summary>
        /// <value>The expire</value>
        public int DefaultExpiration { get; set; } = 0;

    }
}
