
using jfYu.Core.Cache;
using jfYu.Core.Office;
using Microsoft.Extensions.DependencyInjection;

namespace xUnitTestCore
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCacheService();
            services.AddJfYuExcel();
            services.AddJfYuWord(); 
        }
    }
}
