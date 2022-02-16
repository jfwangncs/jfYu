using Autofac;
using jfYu.Core.Cache;
using jfYu.Core.Common.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTest4._7.Cache
{

    public class CacheConstructorClass
    {
        private ICache Cache;
        public CacheConstructorClass(ICache cache)
        {
            Cache = cache;
        }

        public bool IsIsNull()
        {
            return Cache == null;
        }
    }

    public class CachepPropertyCalss
    {
        public ICache Cache { get; set; }

        public bool IsIsNull()
        {
            return Cache == null;
        }
    }

    [TestClass]
    public class TestCacheRedis
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheRedis()
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


        [TestMethod]
        public void TestIoc()
        {

            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.IsFalse(ccc.IsIsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.IsFalse(cpc.IsIsNull());
            Assert.IsNotNull(CacheService);
            Assert.AreEqual(CacheType.Redis, CacheService.CacheType);
        }

        [TestMethod]
        public void TestHas()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.HasAsync("tk3").Result);
            Assert.IsTrue(CacheService.Has("tk4"));
            Assert.IsTrue(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsFalse(CacheService.Has("tk3"));
            Assert.IsFalse(CacheService.HasAsync("tk3").Result);
            Assert.IsFalse(CacheService.Has("tk4"));
            Assert.IsFalse(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.IsFalse(CacheService.Has("tk2"));
            Assert.IsFalse(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4" });
        }

        [TestMethod]
        public void TestAdd()
        {

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.IsTrue(CacheService.Add("tk1", "tv1"));
            Assert.IsFalse(CacheService.Add("tk1", "tv11"));
            Assert.IsTrue(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.Has("tk2"));

            Assert.IsTrue(CacheService.Add("tk3", "tv3", 3));
            Assert.IsTrue(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.Has("tk4"));

            Assert.IsTrue(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.IsTrue(CacheService.Has("tk5"));
            Assert.IsTrue(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [TestMethod]
        public void TestRemove()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.IsTrue(CacheService.Remove("tk1"));
            Assert.IsFalse(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.AreEqual(0, dlc1);
            Assert.AreEqual(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.AreEqual(1, dlc3);
            Assert.AreEqual(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [TestMethod]
        public void TestGet()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.IsNull(CacheService.Get<TestModel>("n"));
            Assert.IsNull(CacheService.GetAsync<TestModel>("n").Result);
            Assert.IsNull(CacheService.GetString("n"));
            Assert.IsNull(CacheService.GetStringAsync("n").Result);
            Assert.IsNull(CacheService.Get("n"));
            Assert.IsNull(CacheService.GetAsync("n").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.AreEqual("tv1", CacheService.GetString("tk1"));
            Assert.AreEqual("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.AreEqual("tv1", CacheService.Get<string>("tk1"));
            Assert.AreEqual("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.IsNull(CacheService.Get("tk2"));
            Assert.IsNull(CacheService.GetAsync("tk2").Result);

            Assert.AreEqual(1234, CacheService.GetInt("tk3"));
            Assert.AreEqual(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.AreEqual(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.AreEqual(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.AreEqual(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.AreEqual(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.AreEqual(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.AreEqual(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);


            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.AreEqual("tv1", result["tk1"].ToString());
            Assert.IsNull(result["tk2"]);
            Assert.AreEqual("1234", result["tk3"].ToString());
            Assert.AreEqual("12345.7", result["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);


            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.AreEqual("tv1", result1["tk1"].ToString());
            Assert.IsNull(result1["tk2"]);
            Assert.AreEqual("1234", result1["tk3"].ToString());
            Assert.AreEqual("12345.7", result1["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);


            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [TestMethod]
        public void TestReplace()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.IsFalse(CacheService.Replace("tk7", "tv1"));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", 3));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.IsTrue(CacheService.Replace("tk1", "tv11"));
            Assert.IsTrue(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.IsTrue(CacheService.Replace("tk3", "tv33", 3));
            Assert.IsTrue(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.IsTrue(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.AreEqual("tv33", CacheService.Get("tk3"));
            Assert.AreEqual("tv44", CacheService.Get("tk4"));
            Assert.AreEqual("tv55", CacheService.Get("tk5"));
            Assert.AreEqual("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.IsNull(CacheService.Get("tk3"));
            Assert.IsNull(CacheService.Get("tk4"));
            Assert.IsNull(CacheService.Get("tk5"));
            Assert.IsNull(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }


    [TestClass]
    public class TestCacheMemory
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheMemory()
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


        [TestMethod]
        public void TestIoc()
        {

            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.IsFalse(ccc.IsIsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.IsFalse(cpc.IsIsNull());
            Assert.IsNotNull(CacheService);
            Assert.AreEqual(CacheType.Memory, CacheService.CacheType);
        }

        [TestMethod]
        public void TestHas()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.HasAsync("tk3").Result);
            Assert.IsTrue(CacheService.Has("tk4"));
            Assert.IsTrue(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsFalse(CacheService.Has("tk3"));
            Assert.IsFalse(CacheService.HasAsync("tk3").Result);
            Assert.IsFalse(CacheService.Has("tk4"));
            Assert.IsFalse(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.IsFalse(CacheService.Has("tk2"));
            Assert.IsFalse(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4" });
        }

        [TestMethod]
        public void TestAdd()
        {

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.IsTrue(CacheService.Add("tk1", "tv1"));
            Assert.IsFalse(CacheService.Add("tk1", "tv11"));
            Assert.IsTrue(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.Has("tk2"));

            Assert.IsTrue(CacheService.Add("tk3", "tv3", 3));
            Assert.IsTrue(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.Has("tk4"));

            Assert.IsTrue(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.IsTrue(CacheService.Has("tk5"));
            Assert.IsTrue(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [TestMethod]
        public void TestRemove()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.IsTrue(CacheService.Remove("tk1"));
            Assert.IsFalse(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.AreEqual(0, dlc1);
            Assert.AreEqual(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.AreEqual(1, dlc3);
            Assert.AreEqual(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [TestMethod]
        public void TestGet()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.IsNull(CacheService.Get<TestModel>("n"));
            Assert.IsNull(CacheService.GetAsync<TestModel>("n").Result);
            Assert.IsNull(CacheService.GetString("n"));
            Assert.IsNull(CacheService.GetStringAsync("n").Result);
            Assert.IsNull(CacheService.Get("n"));
            Assert.IsNull(CacheService.GetAsync("n").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.AreEqual("tv1", CacheService.GetString("tk1"));
            Assert.AreEqual("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.AreEqual("tv1", CacheService.Get<string>("tk1"));
            Assert.AreEqual("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.IsNull(CacheService.Get("tk2"));
            Assert.IsNull(CacheService.GetAsync("tk2").Result);

            Assert.AreEqual(1234, CacheService.GetInt("tk3"));
            Assert.AreEqual(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.AreEqual(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.AreEqual(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.AreEqual(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.AreEqual(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.AreEqual(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.AreEqual(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);


            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.AreEqual("tv1", result["tk1"].ToString());
            Assert.IsNull(result["tk2"]);
            Assert.AreEqual("1234", result["tk3"].ToString());
            Assert.AreEqual("12345.7", result["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);


            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.AreEqual("tv1", result1["tk1"].ToString());
            Assert.IsNull(result1["tk2"]);
            Assert.AreEqual("1234", result1["tk3"].ToString());
            Assert.AreEqual("12345.7", result1["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);


            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [TestMethod]
        public void TestReplace()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.IsFalse(CacheService.Replace("tk7", "tv1"));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", 3));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.IsTrue(CacheService.Replace("tk1", "tv11"));
            Assert.IsTrue(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.IsTrue(CacheService.Replace("tk3", "tv33", 3));
            Assert.IsTrue(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.IsTrue(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.AreEqual("tv33", CacheService.Get("tk3"));
            Assert.AreEqual("tv44", CacheService.Get("tk4"));
            Assert.AreEqual("tv55", CacheService.Get("tk5"));
            Assert.AreEqual("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.IsNull(CacheService.Get("tk3"));
            Assert.IsNull(CacheService.Get("tk4"));
            Assert.IsNull(CacheService.Get("tk5"));
            Assert.IsNull(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }

    [TestClass]
    public class TestCacheDefault
    {
        ICache CacheService;
        IContainer Container;
        public TestCacheDefault()
        {
            var ContainerBuilder = new ContainerBuilder();         
            ContainerBuilder.AddCache();
            ContainerBuilder.AddCacheAsProperties();
            ContainerBuilder.RegisterType<CacheConstructorClass>().AsSelf();
            ContainerBuilder.RegisterType<CachepPropertyCalss>().AsSelf().PropertiesAutowired();
            Container = ContainerBuilder.Build();
            CacheService = Container.Resolve<ICache>();
        }


        [TestMethod]
        public void TestIoc()
        {

            var ccc = Container.Resolve<CacheConstructorClass>();
            Assert.IsFalse(ccc.IsIsNull());
            var cpc = Container.Resolve<CachepPropertyCalss>();
            Assert.IsFalse(cpc.IsIsNull());
            Assert.IsNotNull(CacheService);
            Assert.AreEqual(CacheType.Memory, CacheService.CacheType);
        }

        [TestMethod]
        public void TestHas()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Has(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.HasAsync(null));
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3", 3);
            CacheService.Add("tk4", "tv4", TimeSpan.FromSeconds(3));
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.HasAsync("tk3").Result);
            Assert.IsTrue(CacheService.Has("tk4"));
            Assert.IsTrue(CacheService.HasAsync("tk4").Result);
            Task.Delay(3100).Wait();
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            Assert.IsFalse(CacheService.Has("tk3"));
            Assert.IsFalse(CacheService.HasAsync("tk3").Result);
            Assert.IsFalse(CacheService.Has("tk4"));
            Assert.IsFalse(CacheService.HasAsync("tk4").Result);
            CacheService.Remove("tk1");
            Assert.IsFalse(CacheService.Has("tk1"));
            Assert.IsFalse(CacheService.HasAsync("tk1").Result);
            Assert.IsTrue(CacheService.Has("tk2"));
            Assert.IsTrue(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk2" });
            Assert.IsFalse(CacheService.Has("tk2"));
            Assert.IsFalse(CacheService.HasAsync("tk2").Result);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4" });
        }

        [TestMethod]
        public void TestAdd()
        {

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Add(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.AddAsync(null, null, TimeSpan.FromSeconds(3)));

            Assert.IsTrue(CacheService.Add("tk1", "tv1"));
            Assert.IsFalse(CacheService.Add("tk1", "tv11"));
            Assert.IsTrue(CacheService.AddAsync("tk2", "tv2").Result);
            Assert.IsTrue(CacheService.Has("tk1"));
            Assert.IsTrue(CacheService.Has("tk2"));

            Assert.IsTrue(CacheService.Add("tk3", "tv3", 3));
            Assert.IsTrue(CacheService.AddAsync("tk4", "tv4", 3).Result);
            Assert.IsTrue(CacheService.Has("tk3"));
            Assert.IsTrue(CacheService.Has("tk4"));

            Assert.IsTrue(CacheService.Add("tk5", "tv5", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.AddAsync("tk6", "tv6", TimeSpan.FromSeconds(3)).Result);
            Assert.IsTrue(CacheService.Has("tk5"));
            Assert.IsTrue(CacheService.Has("tk6"));

            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });

        }

        [TestMethod]
        public void TestRemove()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Remove(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveAsync(null));

            Assert.ThrowsException<ArgumentNullException>(() => CacheService.RemoveRange(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.RemoveRangeAsync(null));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            CacheService.Add("tk7", "tv7");
            Assert.IsTrue(CacheService.Remove("tk1"));
            Assert.IsFalse(CacheService.RemoveAsync("tk1").Result);
            var dlc1 = CacheService.RemoveRange(new List<string> { "tk2", "tk3" }).Count();
            var dlc2 = CacheService.RemoveRangeAsync(new List<string> { "tk4", "tk5" }).Result.Count();
            Assert.AreEqual(0, dlc1);
            Assert.AreEqual(0, dlc2);
            var dlc3 = CacheService.RemoveRange(new List<string> { "tk1", "tk6" }).Count();
            var dlc4 = CacheService.RemoveRangeAsync(new List<string> { "tk1", "tk7" }).Result.Count();
            Assert.AreEqual(1, dlc3);
            Assert.AreEqual(1, dlc4);
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6", "tk7" });
        }

        [TestMethod]
        public void TestGet()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get<TestModel>(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync<TestModel>(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Get(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetString(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetStringAsync(null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.GetInt(null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.GetIntAsync(null));

            Assert.IsNull(CacheService.Get<TestModel>("n"));
            Assert.IsNull(CacheService.GetAsync<TestModel>("n").Result);
            Assert.IsNull(CacheService.GetString("n"));
            Assert.IsNull(CacheService.GetStringAsync("n").Result);
            Assert.IsNull(CacheService.Get("n"));
            Assert.IsNull(CacheService.GetAsync("n").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("n"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("n"));

            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk3", 1234);
            CacheService.Add("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new TestModel() { Id = 2, Name = "t2" }, new TestModel() { Id = 3, Name = "t3" } };
            CacheService.Add("tk5", t);
            CacheService.Add("tk6", tl);

            Assert.AreEqual("tv1", CacheService.GetString("tk1"));
            Assert.AreEqual("tv1", CacheService.GetStringAsync("tk1").Result);
            Assert.AreEqual("tv1", CacheService.Get<string>("tk1"));
            Assert.AreEqual("tv1", CacheService.GetAsync<string>("tk1").Result);

            Assert.IsNull(CacheService.Get("tk2"));
            Assert.IsNull(CacheService.GetAsync("tk2").Result);

            Assert.AreEqual(1234, CacheService.GetInt("tk3"));
            Assert.AreEqual(1234, CacheService.GetIntAsync("tk3").Result);
            Assert.ThrowsException<FormatException>(() => CacheService.GetInt("tk1"));
            Assert.ThrowsExceptionAsync<FormatException>(async () => await CacheService.GetIntAsync("tk1"));

            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.Get("tk4").ToString()));
            Assert.AreEqual(12345.7M, decimal.Parse(CacheService.GetAsync("tk4").Result.ToString()));

            Assert.AreEqual(t.Id, CacheService.Get<TestModel>("tk5").Id);
            Assert.AreEqual(t.Name, CacheService.Get<TestModel>("tk5").Name);
            Assert.AreEqual(t.Id, CacheService.GetAsync<TestModel>("tk5").Result.Id);
            Assert.AreEqual(t.Name, CacheService.GetAsync<TestModel>("tk5").Result.Name);

            Assert.AreEqual(tl[0].Id, CacheService.Get<List<TestModel>>("tk6")[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.Get<List<TestModel>>("tk6")[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.Get<List<TestModel>>("tk6")[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.Get<List<TestModel>>("tk6")[1].Name);

            Assert.AreEqual(tl[0].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Id);
            Assert.AreEqual(tl[0].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[0].Name);
            Assert.AreEqual(tl[1].Id, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Id);
            Assert.AreEqual(tl[1].Name, CacheService.GetAsync<List<TestModel>>("tk6").Result[1].Name);


            var result = CacheService.GetRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
            Assert.AreEqual("tv1", result["tk1"].ToString());
            Assert.IsNull(result["tk2"]);
            Assert.AreEqual("1234", result["tk3"].ToString());
            Assert.AreEqual("12345.7", result["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result["tk6"].ToString())[1].Id);


            var result1 = CacheService.GetRangeAsync(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" }).Result;
            Assert.AreEqual("tv1", result1["tk1"].ToString());
            Assert.IsNull(result1["tk2"]);
            Assert.AreEqual("1234", result1["tk3"].ToString());
            Assert.AreEqual("12345.7", result1["tk4"].ToString());
            Assert.AreEqual(t.Id, JsonConvert.DeserializeObject<TestModel>(result1["tk5"].ToString()).Id);
            Assert.AreEqual(tl[1].Id, JsonConvert.DeserializeObject<List<TestModel>>(result1["tk6"].ToString())[1].Id);


            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }

        [TestMethod]
        public void TestReplace()
        {
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x"));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x"));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, "x", TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync("x", null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, 3));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, 3));
            Assert.ThrowsException<ArgumentNullException>(() => CacheService.Replace(null, null, TimeSpan.FromSeconds(3)));
            Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await CacheService.ReplaceAsync(null, null, TimeSpan.FromSeconds(3)));
            CacheService.Add("tk1", "tv1");
            CacheService.Add("tk2", "tv2");
            CacheService.Add("tk3", "tv3");
            CacheService.Add("tk4", "tv4");
            CacheService.Add("tk5", "tv5");
            CacheService.Add("tk6", "tv6");
            Assert.IsFalse(CacheService.Replace("tk7", "tv1"));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1").Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", 3));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", 3).Result);
            Assert.IsFalse(CacheService.Replace("tk7", "tv1", TimeSpan.FromSeconds(3)));
            Assert.IsFalse(CacheService.ReplaceAsync("tk7", "tv1", TimeSpan.FromSeconds(3)).Result);

            Assert.IsTrue(CacheService.Replace("tk1", "tv11"));
            Assert.IsTrue(CacheService.ReplaceAsync("tk2", "tv22").Result);
            Assert.IsTrue(CacheService.Replace("tk3", "tv33", 3));
            Assert.IsTrue(CacheService.ReplaceAsync("tk4", "tv44", 3).Result);
            Assert.IsTrue(CacheService.Replace("tk5", "tv55", TimeSpan.FromSeconds(3)));
            Assert.IsTrue(CacheService.ReplaceAsync("tk6", "tv66", TimeSpan.FromSeconds(3)).Result);

            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.AreEqual("tv33", CacheService.Get("tk3"));
            Assert.AreEqual("tv44", CacheService.Get("tk4"));
            Assert.AreEqual("tv55", CacheService.Get("tk5"));
            Assert.AreEqual("tv66", CacheService.Get("tk6"));
            Task.Delay(3100).Wait();
            Assert.AreEqual("tv11", CacheService.Get("tk1"));
            Assert.AreEqual("tv22", CacheService.Get("tk2"));
            Assert.IsNull(CacheService.Get("tk3"));
            Assert.IsNull(CacheService.Get("tk4"));
            Assert.IsNull(CacheService.Get("tk5"));
            Assert.IsNull(CacheService.Get("tk6"));
            CacheService.RemoveRange(new List<string> { "tk1", "tk2", "tk3", "tk4", "tk5", "tk6" });
        }
    }
}
