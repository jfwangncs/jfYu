using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace jfYu.Core.Cache.Configurations.Memory
{
    public class MemoryOptionsExtensions(Action<BaseOptions> configure) : IOptionsExtension
    { /// <summary>
      /// The configure.
      /// </summary>
        private readonly Action<BaseOptions> _configure = configure;

        public void AddServices(IServiceCollection services)
        {
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            services.Configure(_configure);
            services.AddDistributedMemoryCache();
        }
    }
}
