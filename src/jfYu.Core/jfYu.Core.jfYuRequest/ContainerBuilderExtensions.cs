using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;

namespace jfYu.Core.jfYuRequest
{

    /// <summary>
    ///  Adds http request services extensions
    /// </summary>
    public static class ContainerBuilderExtensions
    {

        /// <summary>
        /// Adds and configures the http web request service to the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the DbContext service to.</param>
        /// <param name="filter">The log filter.</param>   
        public static void AddJfYuHttpRequestService(this IServiceCollection services, Action<LogFilter>? filter = null)
        {
            services.AddScoped<IJfYuRequest, JfYuHttpRequest>();
            var logFilter = new LogFilter();
            filter?.Invoke(logFilter);
            services.AddSingleton(logFilter);

        }


#if NETCORE

        /// <summary>
        ///  Adds and configures the http client service to the dependency injection container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the DbContext service to.</param>
        /// <param name="httpClientHandler">The http client handler</param>
        /// <param name="filter">The log filter.</param>   
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
