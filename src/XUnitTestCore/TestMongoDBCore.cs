using Autofac;
using jfYu.Core.Common.Configurations;
using jfYu.Core.Common.Pagination;
using jfYu.Core.MongoDB;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace xUnitTestCore.MongoDBCore
{
    public class Data : MongoEntity
    {
        public string name;
        public string age;
        public string sex;

    }
    public class TestMongoDBCore
    {
        MongoDBService mongoDBService;
        public TestMongoDBCore()
        {
            var builder = new ConfigurationBuilder()
             .AddConfigurationFile("MongoDB.json", optional: true, reloadOnChange: true);
            var cb = new ContainerBuilder();
            cb.AddMongoDB();
            var con = cb.Build();
            mongoDBService = con.Resolve<MongoDBService>();

        }

        [Fact]
        public void TestIoc()
        {
            Assert.NotNull(mongoDBService);
        }

        [Fact]
        public void TestAdd()
        {

            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };
            Data d3 = new Data() { name = "姓名3", age = "20", sex = "女" };
            Data d4 = new Data() { name = "姓名4", age = "21", sex = "女" };
            Data d5 = new Data() { name = "姓名5", age = "22", sex = "女" };
            Data d6 = new Data() { name = "姓名6", age = "23", sex = "女" };


            d1 = mongoDBService.Insert(d1);
            var d11 = mongoDBService.QueryOne<Data>(q => q.age == "18");
            var d12 = mongoDBService.QueryOne<Data>(q => q.Id == d1.Id);
            Assert.Equal("姓名1", d1.name);
            Assert.Equal("姓名1", d1.name);


            mongoDBService.InsertAsync(d2).Wait();
            var d22 = mongoDBService.QueryOneAsync<Data>(q => q.age == "19").Result;
            Assert.Equal("姓名2", d22.name);


            mongoDBService.InsertBatch(new List<Data>() { d3, d4 });
            var d33 = mongoDBService.QueryOne<Data>(q => q.name == "姓名3");
            Assert.Equal("姓名3", d33.name);
            var d44 = mongoDBService.QueryOne<Data>(q => q.name == "姓名4");
            Assert.Equal("姓名4", d44.name);

            mongoDBService.InsertBatchAsync(new List<Data>() { d5, d6 }).Wait();
            var d55 = mongoDBService.QueryOne<Data>(q => q.name == "姓名5");
            Assert.Equal("姓名5", d55.name);
            var d66 = mongoDBService.QueryOne<Data>(q => q.name == "姓名6");
            Assert.Equal("姓名6", d66.name);

            mongoDBService.Delete<Data>(d11.Id.ToString());
            mongoDBService.Delete<Data>(d22.Id.ToString());
            mongoDBService.Delete<Data>(d33.Id.ToString());
            mongoDBService.Delete<Data>(d44.Id.ToString());
            mongoDBService.Delete<Data>(d55.Id.ToString());
            mongoDBService.Delete<Data>(d66.Id.ToString());

        }

        [Fact]
        public void TestDelete()
        {
            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };
            Data d3 = new Data() { name = "姓名3", age = "20", sex = "男" };
            Data d4 = new Data() { name = "姓名4", age = "21", sex = "男" };


            mongoDBService.InsertBatch(new List<Data>() { d1, d2 });
            var d11 = mongoDBService.QueryOne<Data>(q => q.name == "姓名1");
            Assert.Equal("姓名1", d11.name);
            var d22 = mongoDBService.QueryOne<Data>(q => q.name == "姓名2");
            Assert.Equal("姓名2", d22.name);

            mongoDBService.Delete<Data>(d11.Id.ToString());
            mongoDBService.DeleteAsync<Data>(d22.Id.ToString()).Wait();

            var d111 = mongoDBService.QueryOne<Data>(q => q.name == "姓名1");
            Assert.Null(d111);
            var d222 = mongoDBService.QueryOne<Data>(q => q.name == "姓名2");
            Assert.Null(d222);
            var d33 = mongoDBService.Insert(d3);
            var d44 = mongoDBService.Insert(d4);
            mongoDBService.SoftDelete<Data>(d33.Id.ToString());
            mongoDBService.SoftDeleteAsync<Data>(d44.Id.ToString()).Wait();

            var d333 = mongoDBService.QueryOne<Data>(q => q.name == "姓名3");
            Assert.Equal("姓名3", d333.name);
            Assert.Equal(0, d333.State);
            var d444 = mongoDBService.QueryOne<Data>(q => q.name == "姓名4");
            Assert.Equal("姓名4", d444.name);
            Assert.Equal(0, d444.State);

            mongoDBService.Delete<Data>(d33.Id.ToString());
            mongoDBService.DeleteAsync<Data>(d44.Id.ToString()).Wait();


        }

        [Fact]
        public void TestModify()
        {
            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };


            mongoDBService.InsertBatch(new List<Data>() { d1, d2 });
            var d11 = mongoDBService.QueryOne<Data>(q => q.name == "姓名1");
            Assert.Equal("姓名1", d11.name);
            var d22 = mongoDBService.QueryOne<Data>(q => q.name == "姓名2");
            Assert.Equal("姓名2", d22.name);
            mongoDBService.Modify<Data>("11", "name111", "姓名1改后1111");
            mongoDBService.Modify<Data>(d11.Id.ToString(), "name", "姓名1改后");
            mongoDBService.ModifyAsync<Data>(d22.Id.ToString(), "name", "姓名2改后");

            var d111 = mongoDBService.QueryOne<Data>(q => q.Id == d11.Id);
            Assert.Equal("姓名1改后", d111.name);
            var d222 = mongoDBService.QueryOne<Data>(q => q.Id == d22.Id);
            Assert.Equal("姓名2改后", d222.name);

            mongoDBService.Delete<Data>(d11.Id.ToString());
            mongoDBService.Delete<Data>(d22.Id.ToString());
        }

        [Fact]
        public void TestUpdate()
        {
            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };


            mongoDBService.InsertBatch(new List<Data>() { d1, d2 });
            var d11 = mongoDBService.QueryOne<Data>(q => q.name == "姓名1");
            Assert.Equal("姓名1", d11.name);
            var d22 = mongoDBService.QueryOne<Data>(q => q.name == "姓名2");
            Assert.Equal("姓名2", d22.name);


            d11.name = "姓名1改后";
            mongoDBService.Update(d11);
            d22.name = "姓名2改后";
            mongoDBService.UpdateAsync(d22).Wait();


            var d111 = mongoDBService.QueryOne<Data>(q => q.Id == d11.Id);
            Assert.Equal("姓名1改后", d111.name);
            var d222 = mongoDBService.QueryOne<Data>(q => q.Id == d22.Id);
            Assert.Equal("姓名2改后", d222.name);

            mongoDBService.Delete<Data>(d11.Id.ToString());
            mongoDBService.Delete<Data>(d22.Id.ToString());
        }

        [Fact]
        public void TestQueryList()
        {
            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };
            Data d3 = new Data() { name = "姓名3", age = "20", sex = "女" };
            Data d4 = new Data() { name = "姓名4", age = "21", sex = "女" };
            Data d5 = new Data() { name = "姓名5", age = "22", sex = "女" };
            Data d6 = new Data() { name = "姓名6", age = "23", sex = "女" };

            mongoDBService.InsertBatch(new List<Data>() { d1, d2, d3, d4, d5, d6 });

            var lc = mongoDBService.QueryCollection<Data>(q => q.sex == "女").Count();
            var lc1 = mongoDBService.QueryCollection<Data>(q => q.sex == "男").Count();
            Assert.Equal(4, lc);
            Assert.Equal(2, lc1);
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名1").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名2").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名3").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名4").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名5").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名6").Id.ToString());
        }

        [Fact]
        public void TestPaging()
        {
            Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
            Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };
            Data d3 = new Data() { name = "姓名3", age = "20", sex = "女" };
            Data d4 = new Data() { name = "姓名4", age = "21", sex = "女" };
            Data d5 = new Data() { name = "姓名5", age = "22", sex = "女" };


            mongoDBService.InsertBatch(new List<Data>() { d1, d2, d3, d4, d5 });

            var lc = mongoDBService.QueryCollection<Data>(q => q.sex == "女").ToPaging(new QueryModel() { PageIndex = 1, PageSize = 2 });
            var lc1 = mongoDBService.QueryCollectionAsync<Data>().Result.ToPaging(new QueryModel() { PageIndex = 1, PageSize = 2 });
            Assert.Equal(2, lc.TotalPages);
            Assert.Equal(3, lc1.TotalPages);
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名1").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名2").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名3").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名4").Id.ToString());
            mongoDBService.Delete<Data>(mongoDBService.QueryOne<Data>(q => q.name == "姓名5").Id.ToString());
        }
    }


}
