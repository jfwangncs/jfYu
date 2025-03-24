using jfYu.Core.Data.Constant;
using jfYu.Core.Data.Extension;
using jfYu.Core.Data.Service;
using jfYu.Core.Test.Models.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace jfYu.Core.Test.Data
{
    [Collection("Data")]
    public class DataBaseTest
    {
        public class NullSetupActionExpectData : TheoryData<Action<JfYuDatabaseConfig>?>
        {
            public NullSetupActionExpectData()
            {
                Add(null);
                Add(q => { });
                Add(q => { q.ConnectionString = ""; });
                Add(q => { q.ConnectionString = null!; });
                Add(q => { q.ConnectionString = "     "; });
                Add(q => { q.ConnectionString = "test"; q.ReadOnlyDatabases = [new() { ConnectionString = "" }]; });
            }
        }
        [Theory]
        [ClassData(typeof(NullSetupActionExpectData))]
        public void AddRedisService_SetupActionIsNull_ThrowsException(Action<JfYuDatabaseConfig> action)
        {
            var services = new ServiceCollection();
            Assert.Throws<ArgumentNullException>(() => services.AddJfYuDbContextService<DataContext>(action));
        }

        [Fact]
        public void AddRedisService_MainDatabase_Correctly()
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.ConnectionString = "server=127.0.0.1;Database=Test;uid=Test;pwd=test;";
            });
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<DataContext>();
            var useService = serviceProvider.GetService<IService<User, DataContext>>();

            Assert.NotNull(dbContext);
            Assert.NotNull(useService);
        }

        [Fact]
        public void AddRedisService_MainDatabase1_Correctly()
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.ConnectionString = "server=127.0.0.1;Database=Test;uid=Test;pwd=test;";
                q.ReadOnlyDatabases = [];
            });
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<DataContext>();
            var useService = serviceProvider.GetService<IService<User, DataContext>>();

            Assert.NotNull(dbContext);
            Assert.NotNull(useService);
            Assert.Contains("127.0.0.1", useService.Context.Database.GetConnectionString());
            Assert.Contains("127.0.0.1", useService.ReadonlyContext.Database.GetConnectionString());
        }

        [Fact]
        public void AddRedisService_MainDatabaseWithReadOnly1_Correctly()
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.ConnectionString = "server=127.0.0.1;Database=Test;uid=Test;pwd=test;";
                q.ReadOnlyDatabases = [new() { ConnectionString = "server=127.0.0.2;Database=Test;uid=Test;pwd=test;" }];
            });
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<DataContext>();
            var dbContextReadOnly0 = serviceProvider.GetKeyedService<DataContext>($"JfYuReadOnly0");
            var useService = serviceProvider.GetService<IService<User, DataContext>>();
            Assert.NotNull(dbContext);
            Assert.NotNull(useService);
            Assert.NotNull(dbContextReadOnly0);
            Assert.Contains("127.0.0.2", dbContextReadOnly0.Database.GetConnectionString());

            Assert.Contains("127.0.0.1", useService.Context.Database.GetConnectionString());
            Assert.Contains("127.0.0.2", useService.ReadonlyContext.Database.GetConnectionString());
        }

        [Fact]
        public void AddRedisService_MainDatabaseWithReadOnly2_Correctly()
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.ConnectionString = "server=127.0.0.1;Database=Test;uid=Test;pwd=test;";
                q.ReadOnlyDatabases = [
                    new () { DatabaseType = DatabaseType.SqlServer, ConnectionString = "server=127.0.0.2;Database=Test;uid=Test;pwd=test;" },
                    new () { DatabaseType = DatabaseType.SqlServer, ConnectionString = "server=127.0.0.3;Database=Test;uid=Test;pwd=test;" } ];
            });
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<DataContext>();
            var dbContextReadOnly0 = serviceProvider.GetKeyedService<DataContext>("JfYuReadOnly0");
            var dbContextReadOnly1 = serviceProvider.GetKeyedService<DataContext>("JfYuReadOnly1");
            var useService = serviceProvider.GetService<IService<User, DataContext>>();
            Assert.NotNull(dbContext);
            Assert.NotNull(useService);
            Assert.NotNull(dbContextReadOnly0);
            Assert.Contains("127.0.0.2", dbContextReadOnly0.Database.GetConnectionString());
            Assert.NotNull(dbContextReadOnly1);
            Assert.Contains("127.0.0.3", dbContextReadOnly1.Database.GetConnectionString());

            Assert.Contains("127.0.0.1", useService.Context.Database.GetConnectionString());
            Assert.True(useService.ReadonlyContext.Database.GetConnectionString()!.Contains("127.0.0.2") || useService.ReadonlyContext.Database.GetConnectionString()!.Contains("127.0.0.3"));
        }

        [Fact]
        public void AddRedisService_MainDatabaseWithReadOnly3_Correctly()
        {
            var services = new ServiceCollection();
            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.ConnectionString = "server=127.0.0.1;Database=Test;uid=Test;pwd=test;";
                q.ReadOnlyDatabases = [
                    new () { DatabaseType = DatabaseType.SqlServer, ConnectionString = "server=127.0.0.2;Database=Test;uid=Test;pwd=test;" },
                    new () { DatabaseType = DatabaseType.SqlServer, ConnectionString = "server=127.0.0.3;Database=Test;uid=Test;pwd=test;" },
                    new () { DatabaseType = DatabaseType.Sqlite, ConnectionString = "server=127.0.0.4;Database=Test;uid=Test;pwd=test;" }];
            });
            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<DataContext>();
            var dbContextReadOnly0 = serviceProvider.GetKeyedService<DataContext>("JfYuReadOnly0");
            var dbContextReadOnly1 = serviceProvider.GetKeyedService<DataContext>("JfYuReadOnly1");
            var dbContextReadOnly2 = serviceProvider.GetKeyedService<DataContext>("JfYuReadOnly2");
            var useService = serviceProvider.GetService<IService<User, DataContext>>();
            Assert.NotNull(dbContext);
            Assert.NotNull(useService);
            Assert.NotNull(dbContextReadOnly0);
            Assert.Contains("127.0.0.2", dbContextReadOnly0.Database.GetConnectionString());
            Assert.NotNull(dbContextReadOnly1);
            Assert.Contains("127.0.0.3", dbContextReadOnly1.Database.GetConnectionString());
            Assert.NotNull(dbContextReadOnly2);
            Assert.Contains("127.0.0.4", dbContextReadOnly2.Database.GetConnectionString());

            Assert.Contains("127.0.0.1", useService.Context.Database.GetConnectionString());
            Assert.True(useService.ReadonlyContext.Database.GetConnectionString()!.Contains("127.0.0.2") || useService.ReadonlyContext.Database.GetConnectionString()!.Contains("127.0.0.3") || useService.ReadonlyContext.Database.GetConnectionString()!.Contains("127.0.0.4"));
        }

    }
}
