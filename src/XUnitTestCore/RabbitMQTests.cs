using jfYu.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace xUnitTestCore
{

    public class RabbitMQTests
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

