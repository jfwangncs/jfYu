using Autofac;
using jfYu.Core.Configuration;
using jfYu.Core.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore.Redis
{
    public class RedisTest
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }
    public class TestRedisCore
    {
        [Fact]
        [Trait("Env", "Development")]
        public void TestSetString()
        {
            var builder = WebApplication.CreateBuilder(args);

            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder().AddConfigurationFile("CacheRedis.Production.json");
            builder.Build();
            ContainerBuilder.AddRedisService();
            var c = ContainerBuilder.Build();
            var redis = c.Resolve<RedisService>();
            redis.Set("stringKey1", "stringValue1");
            redis.SetAsync("stringKey2", "stringValue2").Wait();
            redis.Set("stringKey3", "stringValue3", TimeSpan.FromSeconds(2));
            redis.SetAsync("stringKey4", "stringValue4", TimeSpan.FromSeconds(2)).Wait();
            redis.Set("stringKey5", "stringValue5", TimeSpan.FromSeconds(6));
            redis.SetAsync("stringKey6", "stringValue6", TimeSpan.FromSeconds(6)).Wait();
            Task.Delay(3000).Wait();
            var v1 = redis.Get("stringKey1");
            var v2 = redis.GetAsync("stringKey2").Result;
            var v3 = redis.GetAsync("stringKey3").Result;
            var v4 = redis.Get("stringKey4");
            var v5 = redis.Get("stringKey5");
            var v6 = redis.GetAsync("stringKey6").Result;
            Assert.Equal("stringValue1", v1);
            Assert.Equal("stringValue2", v2);
            Assert.Null(v3);
            Assert.Null(v4);
            Assert.Equal("stringValue5", v5);
            Assert.Equal("stringValue6", v6);
            Task.Delay(4000).Wait();
            v5 = redis.Get("stringKey5");
            v6 = redis.GetAsync("stringKey6").Result;
            Assert.Null(v5);
            Assert.Null(v6);
            redis.Remove("stringKey1");
            redis.Remove("stringKey2");
        }


        [Fact]
        [Trait("Env", "Development")]
        public void TestSetModel()
        {
            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder().AddConfigurationFile("CacheRedis.Production.json");
            builder.Build();
            ContainerBuilder.AddRedisService();
            var c = ContainerBuilder.Build();
            var redis = c.Resolve<RedisService>();

            List<RedisTest>[] ModelValues = new List<RedisTest>[6];
            for (int i = 0; i < 6; i++)
            {
                List<RedisTest> temp = new List<RedisTest>();
                for (int j = 0; j < 3; j++)
                {
                    temp.Add(new RedisTest() { Name = $"Name{i}{j}", Age = int.Parse($"{i}{j}"), Address = $"Address{i}{j}" });
                }
                ModelValues[i] = temp;
            }
            JsonConvert.SerializeObject(ModelValues[0]);

            redis.Set("modelKey1", ModelValues[0]);
            redis.SetAsync("modelKey2", ModelValues[1]).Wait();
            redis.Set("modelKey3", ModelValues[2], TimeSpan.FromSeconds(2));
            redis.SetAsync("modelKey4", ModelValues[3], TimeSpan.FromSeconds(2)).Wait();
            redis.Set("modelKey5", ModelValues[4], TimeSpan.FromSeconds(6));
            redis.SetAsync("modelKey6", ModelValues[5], TimeSpan.FromSeconds(6)).Wait();
            Task.Delay(3000).Wait();
            var v1 = redis.Get<List<RedisTest>>("modelKey1");
            var v2 = redis.GetAsync<List<RedisTest>>("modelKey2").Result;
            var v3 = redis.GetAsync<List<RedisTest>>("modelKey3").Result;
            var v4 = redis.Get<List<RedisTest>>("modelKey4");
            var v5 = redis.Get<List<RedisTest>>("modelKey5");
            var v6 = redis.GetAsync<List<RedisTest>>("modelKey6").Result;

            Assert.Equal(JsonConvert.SerializeObject(ModelValues[0]), JsonConvert.SerializeObject(v1));
            Assert.Equal(JsonConvert.SerializeObject(ModelValues[1]), JsonConvert.SerializeObject(v2));
            Assert.Null(v3);
            Assert.Null(v4);
            Assert.Equal(JsonConvert.SerializeObject(ModelValues[4]), JsonConvert.SerializeObject(v5));
            Assert.Equal(JsonConvert.SerializeObject(ModelValues[5]), JsonConvert.SerializeObject(v6));
            redis.Remove("modelKey1");
            redis.Remove("modelKey2");
        }
    }
}
