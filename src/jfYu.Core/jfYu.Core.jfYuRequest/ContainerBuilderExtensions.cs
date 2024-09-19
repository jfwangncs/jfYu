using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace jfYu.Core.jfYuRequest
{

    /// <summary>
    /// 
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuHttpRequestService(this IServiceCollection services)
        {
            services.AddTransient<IJfYuRequest, JfYuHttpRequest>();
        }


#if NETCORE
        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuHttpClientService(this IServiceCollection services, HttpClientHandler? httpClientHandler = null)
        {
            services.AddHttpClient("httpclient").ConfigurePrimaryHttpMessageHandler(() =>
            {
                if (httpClientHandler != null)
                    return httpClientHandler;
                return new HttpClientHandler();
            });
            services.AddScoped<IJfYuRequest, JfYuHttpClient>();
        }
#endif
    }

}
