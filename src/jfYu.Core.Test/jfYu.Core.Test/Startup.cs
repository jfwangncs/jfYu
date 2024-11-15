using jfYu.Core.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Test
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {      
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost", Port = 6379 });
            }).UsingNewtonsoft();
           
        }      
    }
}
