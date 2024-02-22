using jfYu.Core.Data;
using jfYu.Core.Data.Extension;
using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace xUnitTestCore
{

   
    public class DataTests
    {
        #region DB test

        public class NullParaData : TheoryData<JfYuDBConfig?>
        {
            public NullParaData()
            {

                Add(null);

                Add(new JfYuDBConfig() { ConnectionString = "" });

                Add(new JfYuDBConfig());
            }
        }

        [Theory]
        [ClassData(typeof(NullParaData))]
        public void TestCreateNull(JfYuDBConfig config)
        {
            
            var services = new ServiceCollection();
            Assert.ThrowsAny<Exception>(() => { services.AddJfYuDbContextService<DataContext>(config); });
        }

        public class OneDBParaData : TheoryData<JfYuDBConfig>
        {
            public OneDBParaData()
            {
                Add(new JfYuDBConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m1.db;" });
            }
        }


        [Theory]
        [ClassData(typeof(OneDBParaData))]
        public void IOCOneDBTest(JfYuDBConfig config)
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(config);
            var serviceProvider = services.BuildServiceProvider();
            var _masterContext = serviceProvider.GetService<DataContext>();
            Assert.NotNull(_masterContext);
            Assert.Contains(config.ConnectionString, _masterContext.Database.GetConnectionString());

            var _readonlyContext = serviceProvider.GetService<ReadonlyDBContext<DataContext>>();
            Assert.NotNull(_readonlyContext);
            Assert.NotNull(_readonlyContext?.Current);
            Assert.Contains(config.ConnectionString, _masterContext.Database.GetConnectionString());
        }


        public class TwoDBParaData : TheoryData<JfYuDBConfig>
        {
            public TwoDBParaData()
            {
                Add(
                   new JfYuDBConfig()
                   {
                       DatabaseType = DatabaseType.Sqlite,
                       ConnectionString = "Data Source= data/m1.db",
                       ReadOnlyConfigs = [new DatabaseConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m2.db" }]
                   });

            }
        }

        [Theory]
        [ClassData(typeof(TwoDBParaData))]
        public void IOCTwoDBTest(JfYuDBConfig config)
        {

            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(config);
            var serviceProvider = services.BuildServiceProvider();
            var _masterContext = serviceProvider.GetService<DataContext>();
            Assert.NotNull(_masterContext);
            Assert.Contains("m1.db", _masterContext.Database.GetConnectionString());



            var _readonlyContext = serviceProvider.GetService<ReadonlyDBContext<DataContext>>();
            Assert.NotNull(_readonlyContext);
            Assert.NotNull(_readonlyContext.Current);
            Assert.Contains("m2.db", _readonlyContext.Current.Database.GetConnectionString());
        }


        public class ThreeParaData : TheoryData<JfYuDBConfig>
        {
            public ThreeParaData()
            {
                Add(new JfYuDBConfig()
                {
                    DatabaseType = DatabaseType.Sqlite,
                    ConnectionString = "Data Source= data/m1.db",
                    ReadOnlyConfigs = [
                        new DatabaseConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m2.db" },
                        new DatabaseConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m3.db" }
                ]
                });

            }
        }

        [Theory]
        [ClassData(typeof(ThreeParaData))]
        public void IOCMany3DBTest(JfYuDBConfig config)
        {

            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(config);
            var serviceProvider = services.BuildServiceProvider();
            var _masterContext = serviceProvider.GetService<DataContext>();
            Assert.NotNull(_masterContext);
            Assert.Contains("m1.db", _masterContext.Database.GetConnectionString());

            var list = new Dictionary<string, int>() { { "m2", 0 }, { "m3", 0 } };

            for (int i = 0; i < 10; i++)
            {

                using var scoped = serviceProvider.CreateScope();
                {

                    var _readonlyContext = scoped.ServiceProvider.GetService<ReadonlyDBContext<DataContext>>();
                    Assert.NotNull(_readonlyContext);
                    Assert.NotNull(_readonlyContext.Current);
                    var readonlyconnstr = _readonlyContext.Current.Database.GetConnectionString() ?? "";
                    if (readonlyconnstr.Contains("m2.db"))
                    {
                        list["m2"] = ++list["m2"];
                    }
                    if (readonlyconnstr.Contains("m3.db"))
                    {
                        list["m3"] = ++list["m3"];
                    }
                    Assert.True(readonlyconnstr.Contains("m2.db") || readonlyconnstr.Contains("m3.db"));
                }
            }
            Assert.True(list["m2"] > 0);
            Assert.True(list["m3"] > 0);
        }
        #endregion
    }

    #region TestDBContext
    public class Company : BaseEntity
    {
        public int Age { get; set; }
        public string? Name { get; set; }
    }

    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options), IJfYuDbContextService
    {
        public DbSet<Company> Companys { get; set; }
    }
    #endregion
}
