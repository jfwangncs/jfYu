using jfYu.Core.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore.Cache
{
    [Collection("Cache")]
    public class TestCache
    {
        [Fact]
        public async Task TestAdd()
        {
            var services = new ServiceCollection();
            services.AddCacheService();
            var serviceProvider = services.BuildServiceProvider();
            var _cacheService = serviceProvider.GetService<ICacheService>();


            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync(null, "x"));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync(null, "x", 3));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync(null, "x", DateTime.Now.AddSeconds(3)));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>("x", null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>("x", null, 3));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>("x", null, DateTime.Now.AddSeconds(3)));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>(null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>(null, null, 3));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync<string>(null, null, DateTime.Now.AddSeconds(3)));

            await _cacheService.AddAsync("tk1", "tv1");
            Assert.Equal("tv1", await _cacheService.GetAsync("tk1"));

            await _cacheService.AddAsync("tk1", "tv12");
            Assert.Equal("tv12", await _cacheService.GetAsync("tk1"));


            await _cacheService.AddAsync("tk2", "tv2", DateTime.Now.AddSeconds(3));
            Assert.Equal("tv2", await _cacheService.GetAsync("tk2"));

            await _cacheService.AddAsync("tk3", "tv3", 3);
            Assert.Equal("tv3", await _cacheService.GetAsync("tk3"));

            await Task.Delay(3000);

            Assert.Null(await _cacheService.GetAsync("tk2"));
            Assert.Null(await _cacheService.GetAsync("tk3"));

            await _cacheService.RemoveAllAsync();
        }

        [Fact]
        public async Task TestRemove()
        {
            var services = new ServiceCollection();
            services.AddCacheService();
            var serviceProvider = services.BuildServiceProvider();
            var _cacheService = serviceProvider.GetService<ICacheService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.RemoveAsync(null));
            await _cacheService.AddAsync("tk1", "tv1");
            await _cacheService.AddAsync("tk2", "tv2");
            await _cacheService.AddAsync("tk3", "tv3");
            await _cacheService.AddAsync("tk4", "tv4");

            await _cacheService.RemoveAsync("tk1");
            Assert.Null(await _cacheService.GetAsync("tk1"));

            await _cacheService.RemoveAsync("tk2");
            Assert.Null(await _cacheService.GetAsync("tk2"));
            Assert.NotNull(await _cacheService.GetAsync("tk3"));
            Assert.NotNull(await _cacheService.GetAsync("tk4"));

            await _cacheService.RemoveAllAsync();
            Assert.Null(await _cacheService.GetAsync("tk2"));
            Assert.Null(await _cacheService.GetAsync("tk3"));
            Assert.Null(await _cacheService.GetAsync("tk4"));
        }

        [Fact]
        public async Task TestGet()
        {
            var services = new ServiceCollection();
            services.AddCacheService();
            var serviceProvider = services.BuildServiceProvider();
            var _cacheService = serviceProvider.GetService<ICacheService>();

            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetAsync<TestModel>(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetIntAsync(null));

            Assert.Null(await _cacheService.GetAsync<TestModel>("n"));
            Assert.Null(await _cacheService.GetAsync("n"));
            await Assert.ThrowsAsync<NullReferenceException>(async () => await _cacheService.GetIntAsync("n"));

            await _cacheService.AddAsync("tk1", "tv1");
            await _cacheService.AddAsync("tk3", 1234);
            await _cacheService.AddAsync("tk4", 12345.7M);
            var t = new TestModel() { Id = 1, Name = "t1" };
            var tl = new List<TestModel> { new() { Id = 2, Name = "t2" }, new() { Id = 3, Name = "t3" } };
            await _cacheService.AddAsync("tk5", t);
            await _cacheService.AddAsync("tk6", tl);

            Assert.Equal("tv1", await _cacheService.GetAsync("tk1"));
            Assert.Equal("tv1", await _cacheService.GetAsync<string>("tk1"));

            Assert.Null(await _cacheService.GetAsync("tk2"));

            Assert.Equal(1234, await _cacheService.GetIntAsync("tk3"));
            await Assert.ThrowsAsync<FormatException>(async () => await _cacheService.GetIntAsync("tk1"));

            Assert.Equal(12345.7M, decimal.Parse(await _cacheService.GetAsync("tk4")));

            Assert.Equal(t.Id, (await _cacheService.GetAsync<TestModel>("tk5")).Id);
            Assert.Equal(t.Name, (await _cacheService.GetAsync<TestModel>("tk5")).Name);


            var tk6 = await _cacheService.GetAsync<List<TestModel>>("tk6");

            Assert.Equal(tl[0].Id, tk6[0].Id);
            Assert.Equal(tl[0].Name, tk6[0].Name);
            Assert.Equal(tl[1].Id, tk6[1].Id);
            Assert.Equal(tl[1].Name, tk6[1].Name);

            await _cacheService.RemoveAllAsync();
        }

    }
}
