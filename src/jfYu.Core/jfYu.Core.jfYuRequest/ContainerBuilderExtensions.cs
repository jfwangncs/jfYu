using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace jfYu.Core.jfYuRequest
{
#if NETSTANDARD21 || NET6_0 || NET7_0 || NET8_0
    /// <summary>
    /// 
    /// </summary>
    public static class ContainerBuilderExtensions
    {
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
    }
#endif
}
