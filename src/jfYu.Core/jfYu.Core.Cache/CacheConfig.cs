namespace jfYu.Core.Cache
{
    public class CacheConfig
    {
        /// <summary>
        /// 缓存类型
        /// </summary>

        public CacheType Type { get; set; } = CacheType.Memory;


        /// <summary>
        /// 前缀
        /// </summary>

        public string KeySuffix { get; set; }

    }
}
