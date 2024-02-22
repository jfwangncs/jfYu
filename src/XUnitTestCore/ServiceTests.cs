using jfYu.Core.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTestCore
{

    public class ServiceTests
    {
        readonly IService<Company, DataContext> _companyService;

        public ServiceTests(IService<Company, DataContext> company) => _companyService = company;

        [Fact]
        public void IocTest()
        {
            Assert.NotNull(_companyService);
            Assert.Contains("m1.db", _companyService.Context.Database.GetConnectionString());
            Assert.Contains("m1.db", _companyService.ReadonlyContext.Database.GetConnectionString());
        }

        [Fact]
        public async Task AddTest()
        {
            Assert.Equal(1, await _companyService.AddAsync(new Company() { Age = 33, Name = "test" }));
            Assert.Equal(3, await _companyService.AddRangeAsync([new Company() { Age = 33, Name = "test" }, new Company() { Age = 22, Name = "test1" }, new Company() { Age = 11, Name = "test2" }]));
        }

        [Fact]
        public async Task UpdateTest()
        {
            var data = await _companyService.GetOneAsync(q => true);
            Assert.NotNull(data);
            data.Name = "test123";
            Assert.Equal(1, await _companyService.UpdateAsync(data));
            Assert.NotNull(await _companyService.GetOneAsync(q => q.Name == "test123"));
        }
        [Fact]
        public async Task UpdatePredicateTest()
        { 
            var data = _companyService.GetList(q => true).ToList();
            var updateNum = await _companyService.UpdateAsync(q => true, x => x.Name = "test124");
            Assert.Equal(data.Count, updateNum);
            Assert.Equal(data.Count, _companyService.GetList(q => q.Name == "test124").Count());

            Assert.Equal(0, await _companyService.UpdateAsync(q => true, null));
            Assert.Equal(0, await _companyService.UpdateAsync(null, x => x.Name = "test124"));
            Assert.Equal(0, await _companyService.UpdateAsync(null, null));
        }

        [Fact]
        public async Task UpdateRangeTest()
        { 
            var data = _companyService.GetList(q => true).ToList();
            data.ForEach(x => x.Name = "test123");

            Assert.Equal(data.Count, await _companyService.UpdateRangeAsync(data));
            Assert.Equal(data.Count, _companyService.GetList(q => q.Name == "test123").Count());
        }


        [Fact]
        public async Task RemoveTest()
        { 
            var data = await _companyService.GetOneAsync(q => true);

            Assert.NotNull(data);
            Assert.Equal(1, await _companyService.RemoveAsync(q => q.ID.Equals(data.ID)));

            data = await _companyService.GetOneAsync(q => q.ID == data.ID);
            Assert.NotNull(data);
            Assert.Equal(0, data.State);
            Assert.Equal(0, await _companyService.RemoveAsync(null));
        }

        [Fact]
        public async Task HardRemoveTest()
        { 
            var data = await _companyService.GetOneAsync(q => true);
            Assert.NotNull(data);
            Assert.Equal(1, await _companyService.HardRemoveAsync(q => q.ID == data.ID));
            Assert.Null(await _companyService.GetOneAsync(q => q.ID == data.ID));

            Assert.Equal(0, await _companyService.HardRemoveAsync(null));
        }

        [Fact]
        public async Task GetOneTest()
        { 
            var data = new Company() { Age = 33, Name = "test" };
            await _companyService.AddAsync(data);
            Assert.NotNull(await _companyService.GetOneAsync(q => q.ID == data.ID));
            Assert.Null(await _companyService.GetOneAsync());
            Assert.Null(await _companyService.GetOneAsync(q => q.State == 33));
        }



        [Fact]
        public async Task GeListTest()
        { 
            await _companyService.HardRemoveAsync(q => true);

            var data = new List<Company>(){
                new () { Age = 33, Name = "test" },
                new () { Age = 11, Name = "test" },
                new () { Age = 11, Name = "test" },
                new () { Age = 11, Name = "test" }};
            await _companyService.AddRangeAsync(data);

            Assert.Equal(data.Count, _companyService.GetList(null).Count());
            Assert.Equal(data.Count, _companyService.GetList().Count());
            Assert.Equal(data.Count(q => q.Age == 11), _companyService.GetList(q => q.Age == 11).Count());
            Assert.Equal(data.Count(q => q.Age == 33), _companyService.GetList(q => q.Age == 33).Count());
        }

        [Fact]
        public async Task GeListT1Test()
        { 
            await _companyService.HardRemoveAsync(q => true);

            var data = new List<Company>(){
                new () { Age = 33, Name = "test" },
                new () { Age = 11, Name = "test" },
                new () { Age = 11, Name = "test" },
                new () { Age = 11, Name = "test" }};
            await _companyService.AddRangeAsync(data);

            Assert.Equal(data.Count, _companyService.GetList(null, q => new ComonayT1() { Age = q.Age }).Count());
            Assert.Equal(data.Count(q => q.Age == 11), _companyService.GetList(q => q.Age == 11, q => new ComonayT1() { Age = q.Age }).Count());
            Assert.Equal(data.Count(q => q.Age == 33), _companyService.GetList(q => q.Age == 33, q => new ComonayT1() { Age = q.Age }).Count());

            Assert.Equal(0, _companyService.GetList<ComonayT1>(null, null).Count());
        }      
    }

    public class ComonayT1
    {
        public int Age { get; set; }
    }
}
