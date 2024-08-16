
using Bogus;
using jfYu.Core.Cache;
using jfYu.Core.Data;
using jfYu.Core.Data.Extension;
using jfYu.Core.Data.Model;
using jfYu.Core.jfYuRequest;
using jfYu.Core.Office;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace xUnitTestCore
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddCacheService();
            services.AddJfYuExcel();
            services.AddJfYuWord();

            services.AddJfYuDbContextService<DataContext>(new JfYuDBConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m1.db" });
            services.AddJfYuHttpClientService();
            Prepardata();
        }

        private static void Prepardata()
        {

            var companyFake = new Faker<Company>()
                 .RuleFor(p => p.Age, f => f.Random.Number(10000))
                 .RuleFor(p => p.Name, f => f.Name.FirstName());
            var companyFakeData = companyFake.GenerateBetween(5, 11);

            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");
            //for excel test
            if (!Directory.Exists("exceltest"))
                Directory.CreateDirectory("exceltest");

            if (File.Exists("data/m1.db"))
                File.Delete("data/m1.db");

            using var conn = new SqliteConnection("Data Source= data/m1.db");
            conn.Open();
            //创建表
            SqliteCommand cmd = new();
            string sql = "CREATE TABLE Companys(Name varchar(20),Id varchar(20),Age int,State int,CreatedTime datetime,UpdatedTime datetime)";
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();


            foreach (var item in companyFakeData)
            {
                //插入数据
                sql = $"INSERT INTO Companys VALUES('{item.Name}','{item.Id.ToString().ToUpper()}',{item.Age},{item.State},'{item.CreatedTime}','{item.UpdatedTime}')";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            conn.Dispose();
            SqliteConnection.ClearAllPools();
        }
    }
}
