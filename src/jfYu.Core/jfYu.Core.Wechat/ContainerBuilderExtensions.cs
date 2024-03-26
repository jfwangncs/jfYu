using jfYu.Core.Wechat.Config;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace jfYu.Core.Wechat
{
    public static class ContainerBuilderExtensions
    {

        #region 小程序
        /// <summary>
        /// 小程序
        /// </summary>
        /// <param name="services"></param>
        public static void AddMiniProgram(this IServiceCollection services,WechatConfig wechatConfig)
        {
            services.AddSingleton(wechatConfig);
            services.AddSingleton<MiniProgram>();
            services.AddHttpClient<HttpClient>(Constant.Mini, q =>
            {
                q.BaseAddress = new Uri("https://api.weixin.qq.com/");
                q.Timeout = TimeSpan.FromSeconds(30);
            });
        }
       

        #endregion

        #region 支付
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="services"></param>
        public static void AddWechatPayment(this IServiceCollection services, PaymentConfig config)
        {
            services.AddSingleton(config);
            services.AddSingleton<WechatPayment>();
            services.AddHttpClient<HttpClient>(Constant.Payment, q =>
            {
                q.BaseAddress = new Uri("https://api.mch.weixin.qq.com/");
                q.Timeout = TimeSpan.FromSeconds(30);
            }).ConfigurePrimaryHttpMessageHandler(q =>
            {
                var handler = new HttpClientHandler()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                };
                var cert = new X509Certificate2(AppContext.BaseDirectory + config.CertPath, config.MchID, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                handler.ClientCertificates.Add(cert);
                handler.SslProtocols = SslProtocols.Tls12;
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                return handler;
            });
        }

        #endregion
    }
}
