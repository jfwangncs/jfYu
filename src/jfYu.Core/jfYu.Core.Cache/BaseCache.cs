namespace jfYu.Core.Cache
{
    public abstract class CacheBase
    {
        public CacheType CacheType { get; protected set; }
        private readonly CacheConfig cacheConfig;
        public CacheBase(CacheConfig cacheConfig)
        {
            this.cacheConfig = cacheConfig;
        }
        protected string GetKey(string key)
        {
            if (string.IsNullOrEmpty(cacheConfig.KeySuffix))
                return $"{CacheType}Cache_{key}";
            else
                return $"{cacheConfig.KeySuffix}_{key}";
        }
    }
}
