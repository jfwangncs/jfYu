using jfYu.Core.Office;
using jfYu.Core.Redis.Extensions;
using jfYu.Core.Redis.Options;
using jfYu.Core.Test.MemoryDB;
using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Test
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.SSL = false;
                options.DbIndex = 1;
                options.Prefix = "Mytest:";
                options.EnableLogs = true;
                options.UsingNewtonsoft(options =>
                {
                    options.MaxDepth = 12;
                });
            });
            services.AddJfYuExcel(q => { q.SheetMaxRecord = 10; });
            services.AddJfYuWord();

            services.AddDbContext<TestDbContext>();
        }
    }
}
