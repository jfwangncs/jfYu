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
        public static void AddJfYuHttpRequestService(this IServiceCollection services, Func<string, string>? requestfilter = null, Func<string, string>? responsefilter = null)
        {
            services.AddScoped<IJfYuRequest, JfYuHttpRequest>();
            var logFilter = new LogFilter();
            if (requestfilter != null)
                logFilter.RequestFunc = requestfilter;
            if (responsefilter != null)
                logFilter.ResponseFunc = responsefilter;
            services.AddSingleton(logFilter);
        }


#if NETCORE
        /// <summary>
        /// injection
        /// </summary>
        /// <param name="services"></param>
        public static void AddJfYuHttpClientService(this IServiceCollection services, Func<HttpClientHandler>? httpClientHandler = null, Func<string, string>? requestfilter = null, Func<string, string>? responsefilter = null)
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
            if (requestfilter != null)
                logFilter.RequestFunc = requestfilter;
            if (responsefilter != null)
                logFilter.ResponseFunc = responsefilter;
            services.AddSingleton(logFilter);
        }
#endif
    }

}
