using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
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
        public static void AddJfYuHttpRequestService(this IServiceCollection services, Action<LogFilter>? filter = null)
        {
            services.AddScoped<IJfYuRequest, JfYuHttpRequest>();
            var logFilter = new LogFilter();
            filter?.Invoke(logFilter);
            services.AddSingleton(logFilter);

        }


#if NETCORE
        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuHttpClientService(this IServiceCollection services, Func<HttpClientHandler>? httpClientHandler = null, Action<LogFilter>? filter = null)
        {
            var build = services.AddHttpClient("httpclient");
            services.AddSingleton<CookieContainer>();
            if (httpClientHandler is not null)
                build.ConfigurePrimaryHttpMessageHandler(httpClientHandler);
            else
            {
                build.ConfigurePrimaryHttpMessageHandler(sp =>
                {
                    var cookieContainer = sp.GetRequiredService<CookieContainer>();
                    return new HttpClientHandler
                    {
                        CookieContainer = cookieContainer,
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
                    };
                });
            }

            services.AddScoped<IJfYuRequest, JfYuHttpClient>();
            var logFilter = new LogFilter();
            filter?.Invoke(logFilter);
            services.AddSingleton(logFilter);

        }
#endif
    }

}
