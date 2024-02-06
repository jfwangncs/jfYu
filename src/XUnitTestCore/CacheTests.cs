using jfYu.Core.Cache;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore.Cache
{
    public class CacheTests(ICacheService cacheService)
    {
        private readonly ICacheService _cacheService = cacheService;

        [Fact]
        public void IOCTest()
        {
            var services = new ServiceCollection();
            services.AddCacheService();
            var serviceProvider = services.BuildServiceProvider();
            var _cacheService = serviceProvider.GetService<ICacheService>();
            Assert.NotNull(_cacheService);

        }

        #region Add

        public class NullParaData : TheoryData<string?, string?>
        {
            public NullParaData()
            {
                Add("", "x");
                Add(null, "x");
            }
        }

        [Theory]
        [ClassData(typeof(NullParaData))]
        public async Task AddNullParaTest(string key, string value)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync(key, value));
        }


        public class NullParaWithDatetimeData : TheoryData<string?, string?, TimeSpan?>
        {
            public NullParaWithDatetimeData()
            {
                Add("", "x", TimeSpan.FromSeconds(3));
                Add(null, "x", TimeSpan.FromSeconds(3));
                Add("", "x", null);
                Add(null, "x", null);
            }
        }
        [Theory]
        [ClassData(typeof(NullParaWithDatetimeData))]
        public async Task AddNullParaWithDatetimeTest(string key, string value, TimeSpan? expiry)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.AddAsync(key, value, expiry));
        }


        public class AddExpectData : TheoryData<string, object?>
        {
            public AddExpectData()
            {
                Add("k1", "v1");
                Add("k2", "1");
                Add("k3", "2.00331");
                Add("k4", 2);
                Add("k5", 1.123131M);
                Add("k6", null);
                Add("k7", "");
            }
        }

        [Theory]
        [ClassData(typeof(AddExpectData))]
        public async Task AddExpectTest(string key, object? value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(value?.ToString(), await _cacheService.GetAsync(key));
            await _cacheService.RemoveAsync(key);
        }

        public class AddStringExpectData : TheoryData<string, string?>
        {
            public AddStringExpectData()
            {
                Add("k1", "v1");
                Add("k2", "1");
                Add("k3", "2.00331");
                Add("k6", null);
                Add("k7", "");
            }
        }

        [Theory]
        [ClassData(typeof(AddStringExpectData))]
        public async Task AddStringExpectTest(string key, string? value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(value?.ToString(), await _cacheService.GetAsync(key));
            await _cacheService.RemoveAsync(key);
        }

        public class AddIntExpectData : TheoryData<string, int?>
        {
            public AddIntExpectData()
            {
                Add("k4", 2);
                Add("k5", 0);
                Add("k5", -1);
                Add("k6", null);
            }
        }

        [Theory]
        [ClassData(typeof(AddIntExpectData))]
        public async Task AddIntExpectTest(string key, int? value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(value?.ToString(), await _cacheService.GetAsync(key));
            await _cacheService.RemoveAsync(key);
        }



        public class AddExpectWithSecondsData : TheoryData<string, object, int>
        {
            public AddExpectWithSecondsData()
            {
                Add("k1", "v1", 2);
                Add("k2", "1", 3);
            }
        }

        [Theory]
        [ClassData(typeof(AddExpectWithSecondsData))]
        public async Task AddExpectWithSecondsTest(string key, object value, int expiry)
        {
            await _cacheService.AddAsync(key, value, TimeSpan.FromSeconds(expiry));
            Assert.Equal(value.ToString(), await _cacheService.GetAsync(key));
            await Task.Delay(expiry * 1000);
            Assert.Null(await _cacheService.GetAsync(key));
        }


        public class CacheTestData : TheoryData<string, TestModel?>
        {
            public CacheTestData()
            {
                Add("k1", new TestModel() { Id = 1, Name = "name1" });
                Add("k2", new TestModel() { Id = 2, Name = "name2" });
                Add("k3", new TestModel() { Id = 3, Name = "" });
                Add("k4", null);
            }
        }

        [Theory]
        [ClassData(typeof(CacheTestData))]
        public async Task AddClassExpectTest(string key, TestModel value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(JsonConvert.SerializeObject(value), JsonConvert.SerializeObject(await _cacheService.GetAsync<TestModel>(key)));
            await _cacheService.RemoveAsync(key);
        }
        #endregion

        #region Remove
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task RemoveNullTest(string key)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.RemoveAsync(key));
        }

        [Theory]
        [InlineData("k1", "v1")]
        [InlineData("k2", "v2")]
        [InlineData("k3", "v3")]
        [InlineData("k4", "v4")]
        public async Task RemoveTest(string key, string value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.NotNull(await _cacheService.GetAsync(key));
            await _cacheService.RemoveAsync(key);
            Assert.Null(await _cacheService.GetAsync(key));
        }

        #endregion

        #region Get

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GeExceptionTest(string key)
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetAsync<TestModel>(key));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetAsync(key));
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await _cacheService.GetIntAsync(key));
        }
        [Theory]
        [InlineData("k1", "")]
        [InlineData("k1", null)]
        public async Task GetNullTest(string key, string value)
        {
            await _cacheService.AddAsync(key, value);
            var v1 = await _cacheService.GetAsync<TestModel>(key);
            Assert.Null(v1);
            var v2 = await _cacheService.GetAsync(key);
            Assert.Equal(value, v2);
            Assert.True(string.IsNullOrEmpty(v2));

            await _cacheService.RemoveAsync(key);
        }


        [Theory]
        [InlineData("k1", null)]
        public async Task GetNullIntTest(string key, string value)
        {
            await _cacheService.AddAsync(key, value);
            var v1 = await _cacheService.GetIntAsync(key);
            Assert.Null(v1);
            await _cacheService.RemoveAsync(key);
        }

        [Theory]
        [InlineData("k1", "test")]
        [InlineData("k1", "@#$%^")]
        [InlineData("k1", "")]
        public async Task GetIntExceptionTest(string key, object value)
        {
            await _cacheService.AddAsync(key, value);
            await Assert.ThrowsAsync<FormatException>(async () => await _cacheService.GetIntAsync(key));
            await _cacheService.RemoveAsync(key);
        }

        public class GetIntData : TheoryData<string, object>
        {
            public GetIntData()
            {
                Add("k1", "-13213");
                Add("k2", "0");
                Add("k3", "6789");
                Add("k4", -21);
                Add("k5", 0);
                Add("k6", 6789);
            }
        }

        [Theory]
        [ClassData(typeof(GetIntData))]
        public async Task GetIntTest(string key, object value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(int.Parse(value.ToString() ?? ""), await _cacheService.GetIntAsync(key));
            await _cacheService.RemoveAsync(key);
        }


        public class GetStringData : TheoryData<string, string>
        {
            public GetStringData()
            {
                Add("k1", "-13213");
                Add("k2", "0");
                Add("k3", "6789");
                Add("k4", "逗我啊打大我打完打啊打");
                Add("k5", "gfwafkadawfafa");
                Add("k5", "~!@#$%^&*()_+<>?{}");
            }
        }

        [Theory]
        [ClassData(typeof(GetStringData))]
        public async Task GetStringTest(string key, string value)
        {
            await _cacheService.AddAsync(key, value);
            Assert.Equal(value, await _cacheService.GetAsync(key));
            await _cacheService.RemoveAsync(key);
        }
        #endregion
    }

}
