using Autofac;
using jfYu.Core.Cache;
using jfYu.Core.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore.Cache
{

    public class CacheConstructorClass
    {
        private ICache Cache;
        public CacheConstructorClass(ICache cache)
        {
            Cache = cache;
        }

        public bool IsNull()
        {
            return Cache == null;
        }
    }

    public class CachepPropertyCalss
    {
        public ICache Cache { get; set; }

        public bool IsNull()
        {
            return Cache == null;
        }
    }

    [Collection("Cache")]
    public class TestCacheRedisCore
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheRedisCore()
        {

            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder()
              .AddConfigurationFile("CacheRedis.json", optional: true, reloadOnChange: true);
            _ = builder.Build();

            ContainerBuilder.AddCache();
            ContainerBuilder.AddCacheAsProperties();
            ContainerBuilder.RegisterType<CacheConstructorClass>().AsSelf();
            ContainerBuilder.RegisterType<CachepPropertyCalss>().AsSelf().PropertiesAutowired();
            Container = ContainerBuilder.Build();
            CacheService = Container.Resolve<ICache>();
        }
        [Fact]
        public void TestIoc()
        {

            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.False(ccc.IsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.False(cpc.IsNull());
            Assert.NotNull(CacheService);
            Assert.Equal(CacheType.Redis, CacheService.CacheType);
        }

        [Fact]
        public void TestHas()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.HasAsync("tk3").Result);
            Assert.True(CacheService.Has("tk4"));
            Assert.True(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.False(CacheService.Has("tk3"));
            Assert.False(CacheService.HasAsync("tk3").Result);
            Assert.False(CacheService.Has("tk4"));
            Assert.False(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.False(CacheService.Has("tk2"));
            Assert.False(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4" });
        }

        [Fact]
        public void TestAdd()
        {

            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.True(CacheService.Add("tk1", "tv1"));
            Assert.False(CacheService.Add("tk1", "tv11"));
            Assert.True(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.Has("tk2"));

            Assert.True(CacheService.Add("tk3", "tv3", 3));
            Assert.True(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.Has("tk4"));

            Assert.True(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.True(CacheService.Has("tk5"));
            Assert.True(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [Fact]
        public void TestRemove()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.Throws<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.True(CacheService.Remove("tk1"));
            Assert.False(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.Equal(0, dlc1);
            Assert.Equal(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.Equal(1, dlc3);
            Assert.Equal(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [Fact]
        public void TestGet()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.Null(CacheService.Get<TestModel>("n"));
            Assert.Null(CacheService.GetAsync<TestModel>("n").Result);
            Assert.Null(CacheService.GetString("n"));
            Assert.Null(CacheService.GetStringAsync("n").Result);
            Assert.Null(CacheService.Get("n"));
            Assert.Null(CacheService.GetAsync("n").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.Equal("tv1", CacheService.GetString("tk1"));
            Assert.Equal("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.Equal("tv1", CacheService.Get<string>("tk1"));
            Assert.Equal("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.Null(CacheService.Get("tk2"));
            Assert.Null(CacheService.GetAsync("tk2").Result);

            Assert.Equal(1234, CacheService.GetInt("tk3"));
            Assert.Equal(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.Equal(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.Equal(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.Equal(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.Equal(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.Equal(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.Equal(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.Equal(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.Equal(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.Equal(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.Equal(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.Equal(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.Equal(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.Equal(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.Equal(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);


            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.Equal("tv1", result["tk1"].ToString());
            Assert.Null(result["tk2"]);
            Assert.Equal("1234", result["tk3"].ToString());
            Assert.Equal("12345.7", result["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);
    

            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.Equal("tv1", result1["tk1"].ToString());
            Assert.Null(result1["tk2"]);
            Assert.Equal("1234", result1["tk3"].ToString());
            Assert.Equal("12345.7", result1["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);


            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [Fact]
        public void TestReplace()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.False(CacheService.Replace("tk7", "tv1"));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.False(CacheService.Replace("tk7", "tv1", 3));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.False(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.True(CacheService.Replace("tk1", "tv11"));
            Assert.True(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.True(CacheService.Replace("tk3", "tv33", 3));
            Assert.True(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.True(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Equal("tv33", CacheService.Get("tk3"));
            Assert.Equal("tv44", CacheService.Get("tk4"));
            Assert.Equal("tv55", CacheService.Get("tk5"));
            Assert.Equal("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Null(CacheService.Get("tk3"));
            Assert.Null(CacheService.Get("tk4"));
            Assert.Null(CacheService.Get("tk5"));
            Assert.Null(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }

    [Collection("Cache")]
    public class TestCacheMemoryCore
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheMemoryCore()
        {
            var ContainerBuilder = new ContainerBuilder();
            var builder = new ConfigurationBuilder()
              .AddConfigurationFile("CacheMemory.json", optional: true, reloadOnChange: true);
            _ = builder.Build();
            ContainerBuilder.AddCache();
            ContainerBuilder.AddCacheAsProperties();
            ContainerBuilder.RegisterType<CacheConstructorClass>().AsSelf();
            ContainerBuilder.RegisterType<CachepPropertyCalss>().AsSelf().PropertiesAutowired();
            Container = ContainerBuilder.Build();
            CacheService = Container.Resolve<ICache>();
        }

        [Fact]
        public void TestIoc()
        {
            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.False(ccc.IsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.False(cpc.IsNull());
            Assert.NotNull(CacheService);
            Assert.Equal(CacheType.Memory, CacheService.CacheType);
        }

        [Fact]
        public void TestHas()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.HasAsync("tk3").Result);
            Assert.True(CacheService.Has("tk4"));
            Assert.True(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.False(CacheService.Has("tk3"));
            Assert.False(CacheService.HasAsync("tk3").Result);
            Assert.False(CacheService.Has("tk4"));
            Assert.False(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.False(CacheService.Has("tk2"));
            Assert.False(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [Fact]
        public void TestAdd()
        {

            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.True(CacheService.Add("tk1", "tv1"));
            Assert.False(CacheService.Add("tk1", "tv11"));
            Assert.True(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.Has("tk2"));

            Assert.True(CacheService.Add("tk3", "tv3", 3));
            Assert.True(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.Has("tk4"));

            Assert.True(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.True(CacheService.Has("tk5"));
            Assert.True(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [Fact]
        public void TestRemove()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.Throws<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.True(CacheService.Remove("tk1"));
            Assert.False(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.Equal(0, dlc1);
            Assert.Equal(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.Equal(1, dlc3);
            Assert.Equal(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [Fact]
        public void TestGet()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.Null(CacheService.Get<TestModel>("n"));
            Assert.Null(CacheService.GetAsync<TestModel>("n").Result);
            Assert.Null(CacheService.GetString("n"));
            Assert.Null(CacheService.GetStringAsync("n").Result);
            Assert.Null(CacheService.Get("n"));
            Assert.Null(CacheService.GetAsync("n").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.Equal("tv1", CacheService.GetString("tk1"));
            Assert.Equal("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.Equal("tv1", CacheService.Get<string>("tk1"));
            Assert.Equal("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.Null(CacheService.Get("tk2"));
            Assert.Null(CacheService.GetAsync("tk2").Result);

            Assert.Equal(1234, CacheService.GetInt("tk3"));
            Assert.Equal(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.Equal(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.Equal(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.Equal(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.Equal(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.Equal(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.Equal(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.Equal(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.Equal(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.Equal(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.Equal(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.Equal(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.Equal(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.Equal(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.Equal(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);

            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.Equal("tv1", result["tk1"].ToString());
            Assert.Null(result["tk2"]);
            Assert.Equal("1234", result["tk3"].ToString());
            Assert.Equal("12345.7", result["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);

            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.Equal("tv1", result1["tk1"].ToString());
            Assert.Null(result1["tk2"]);
            Assert.Equal("1234", result1["tk3"].ToString());
            Assert.Equal("12345.7", result1["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [Fact]
        public void TestReplace()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.False(CacheService.Replace("tk7", "tv1"));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.False(CacheService.Replace("tk7", "tv1", 3));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.False(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.True(CacheService.Replace("tk1", "tv11"));
            Assert.True(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.True(CacheService.Replace("tk3", "tv33", 3));
            Assert.True(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.True(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Equal("tv33", CacheService.Get("tk3"));
            Assert.Equal("tv44", CacheService.Get("tk4"));
            Assert.Equal("tv55", CacheService.Get("tk5"));
            Assert.Equal("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Null(CacheService.Get("tk3"));
            Assert.Null(CacheService.Get("tk4"));
            Assert.Null(CacheService.Get("tk5"));
            Assert.Null(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }

    [Collection("Cache")]
    public class TestCacheDefaultCore
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheDefaultCore()
        {
            var ContainerBuilder = new ContainerBuilder();          
            ContainerBuilder.AddCache();
            ContainerBuilder.AddCacheAsProperties();
            ContainerBuilder.RegisterType<CacheConstructorClass>().AsSelf();
            ContainerBuilder.RegisterType<CachepPropertyCalss>().AsSelf().PropertiesAutowired();
            Container = ContainerBuilder.Build();
            CacheService = Container.Resolve<ICache>();
        }

        [Fact]
        public void TestIoc()
        {

            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.False(ccc.IsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.False(cpc.IsNull());
            Assert.NotNull(CacheService);
            Assert.Equal(CacheType.Memory, CacheService.CacheType);
        }

        [Fact]
        public void TestHas()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.HasAsync("tk3").Result);
            Assert.True(CacheService.Has("tk4"));
            Assert.True(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            Assert.False(CacheService.Has("tk3"));
            Assert.False(CacheService.HasAsync("tk3").Result);
            Assert.False(CacheService.Has("tk4"));
            Assert.False(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.False(CacheService.Has("tk1"));
            Assert.False(CacheService.HasAsync("tk1").Result);
            Assert.True(CacheService.Has("tk2"));
            Assert.True(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.False(CacheService.Has("tk2"));
            Assert.False(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [Fact]
        public void TestAdd()
        {

            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.True(CacheService.Add("tk1", "tv1"));
            Assert.False(CacheService.Add("tk1", "tv11"));
            Assert.True(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.True(CacheService.Has("tk1"));
            Assert.True(CacheService.Has("tk2"));

            Assert.True(CacheService.Add("tk3", "tv3", 3));
            Assert.True(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.True(CacheService.Has("tk3"));
            Assert.True(CacheService.Has("tk4"));

            Assert.True(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.True(CacheService.Has("tk5"));
            Assert.True(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [Fact]
        public void TestRemove()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.Throws<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.True(CacheService.Remove("tk1"));
            Assert.False(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.Equal(0, dlc1);
            Assert.Equal(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.Equal(1, dlc3);
            Assert.Equal(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [Fact]
        public void TestGet()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.Throws<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.Null(CacheService.Get<TestModel>("n"));
            Assert.Null(CacheService.GetAsync<TestModel>("n").Result);
            Assert.Null(CacheService.GetString("n"));
            Assert.Null(CacheService.GetStringAsync("n").Result);
            Assert.Null(CacheService.Get("n"));
            Assert.Null(CacheService.GetAsync("n").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.Equal("tv1", CacheService.GetString("tk1"));
            Assert.Equal("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.Equal("tv1", CacheService.Get<string>("tk1"));
            Assert.Equal("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.Null(CacheService.Get("tk2"));
            Assert.Null(CacheService.GetAsync("tk2").Result);

            Assert.Equal(1234, CacheService.GetInt("tk3"));
            Assert.Equal(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.Throws<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.Equal(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.Equal(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.Equal(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.Equal(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.Equal(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.Equal(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.Equal(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.Equal(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.Equal(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.Equal(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.Equal(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.Equal(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.Equal(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.Equal(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);

            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.Equal("tv1", result["tk1"].ToString());
            Assert.Null(result["tk2"]);
            Assert.Equal("1234", result["tk3"].ToString());
            Assert.Equal("12345.7", result["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);

            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.Equal("tv1", result1["tk1"].ToString());
            Assert.Null(result1["tk2"]);
            Assert.Equal("1234", result1["tk3"].ToString());
            Assert.Equal("12345.7", result1["tk4"].ToString());
            Assert.Equal(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.Equal(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [Fact]
        public void TestReplace()
        {
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.Throws<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.False(CacheService.Replace("tk7", "tv1"));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.False(CacheService.Replace("tk7", "tv1", 3));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.False(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.False(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.True(CacheService.Replace("tk1", "tv11"));
            Assert.True(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.True(CacheService.Replace("tk3", "tv33", 3));
            Assert.True(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.True(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.True(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Equal("tv33", CacheService.Get("tk3"));
            Assert.Equal("tv44", CacheService.Get("tk4"));
            Assert.Equal("tv55", CacheService.Get("tk5"));
            Assert.Equal("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.Equal("tv11", CacheService.Get("tk1"));
            Assert.Equal("tv22", CacheService.Get("tk2"));
            Assert.Null(CacheService.Get("tk3"));
            Assert.Null(CacheService.Get("tk4"));
            Assert.Null(CacheService.Get("tk5"));
            Assert.Null(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }
}
