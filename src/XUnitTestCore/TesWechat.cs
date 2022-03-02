using jfYu.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;
using xUnitTestCore.Cache;
using Autofac;
using jfYu.Core.Wechat;
using jfYu.Core.Wechat.Model.Payment;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace xUnitTestCore.Wechat
{
    public class TestWechat
    {
        [Fact]
        public void TestAuth()
        {
            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder()
              .AddConfigurationFile("Wechat.Production.json", optional: true, reloadOnChange: true);


            ContainerBuilder.AddMiniProgram();
            var Container = ContainerBuilder.Build();
            var service = Container.Resolve<MiniProgram>();
            var session = service.Auth("0934nZ000in2mN15nw000re4s524nZ0X");
            Assert.NotNull(session.Code);


        }
        [Fact]
        public void TestToken()
        {
            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder()
              .AddConfigurationFile("Wechat.Production.json", optional: true, reloadOnChange: true);


            ContainerBuilder.AddMiniProgram();
            var Container = ContainerBuilder.Build();
            var service = Container.Resolve<MiniProgram>();
            var session = service.GetToken();
            Assert.NotNull(session.Token);


        }

        [Fact]
        public void TestUnifiedOrder()
        {
            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder()
              .AddConfigurationFile("Wechat.Production.json", optional: true, reloadOnChange: true);


            ContainerBuilder.AddMiniProgram();
            var Container = ContainerBuilder.Build();
            var service = Container.Resolve<WechatPayment>();

            service.Unify(new UnifiedOrder() { Body = "123", OutTradeNo = "123", TotalFee = 12, TradeType = TradeType.JsApi, Openid = "123" });
        }
    }
}
