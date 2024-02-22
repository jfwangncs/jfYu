using jfYu.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace xUnitTestCore.RabbitMQ
{

    public class RabbitMQTest
    {

        public class NullParaData : TheoryData<RabbitMQConfig?>
        {
            public NullParaData()
            {

                Add(null);

                Add(new RabbitMQConfig() { HostName = "" });

                Add(new RabbitMQConfig());
            }
        }

        [Theory]
        [ClassData(typeof(NullParaData))]

        public void TestCreateNull(RabbitMQConfig config)
        {
            var services = new ServiceCollection();
            Assert.ThrowsAny<Exception>(() => { services.AddRabbitMQService(config); });
        } 
    }
}

