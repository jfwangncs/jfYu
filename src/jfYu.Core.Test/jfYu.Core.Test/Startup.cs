using jfYu.Core.Data.Constant;
using jfYu.Core.Data.Extension;
using jfYu.Core.jfYuRequest;
using jfYu.Core.Office;
using jfYu.Core.RabbitMQ;
using jfYu.Core.Redis.Extensions;
using jfYu.Core.Test.MemoryDB;
using jfYu.Core.Test.Models.Entity;
using jfYu.Core.Test.Models.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Net;

namespace jfYu.Core.Test
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();

            services.AddRabbitMQService((q, e) =>
            {
                configuration.GetSection("RabbitMQ").Bind(q);
                configuration.GetSection("RabbitMQ:RetryPolicy").Bind(e);
            });

            services.AddRedisService(options =>
            {
                configuration.GetSection("Redis").Bind(options);
                options.UsingNewtonsoft(options =>
                {
                    options.MaxDepth = 12;
                });
            });
            services.AddJfYuExcel(q => { q.SheetMaxRecord = 10; });
            services.AddJfYuWord();
            services.AddDbContext<TestDbContext>();

            var dbconfig = configuration.GetSection("JfYuConnectionStrings").Get<JfYuDatabaseConfig>();

            services.AddJfYuDbContextService<DataContext>(options =>
            {
                configuration.GetSection("JfYuConnectionStrings").Bind(options);
            });

            services.AddScoped<IUserService, UserService>();
            InitializeDatabase(dbconfig!.DatabaseType, dbconfig.ConnectionString);

            services.AddJfYuHttpClientService(() =>
            {
                return new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
                };
            });
        }

        public static void InitializeDatabase(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.MySql)
            {
                var builder = new MySqlConnectionStringBuilder(connectionString)
                {
                    Database = string.Empty
                };
                var rawConnection = new MySqlConnection(builder.ConnectionString);
                var cmd = new MySqlCommand
                {
                    Connection = rawConnection
                };
                rawConnection.Open();
                cmd.CommandText = "CREATE DATABASE IF NOT EXISTS Test;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "USE Test;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INT AUTO_INCREMENT PRIMARY KEY,
                        UserName VARCHAR(100) NOT NULL,
                        NickName VARCHAR(100) NOT NULL,
                        DepartmentId INT NULL,
                        State INT NOT NULL DEFAULT 1,
                        CreatedTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        UpdatedTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                    );
                ";
                cmd.ExecuteNonQuery();
                rawConnection.Close();
            }
            else if (databaseType == DatabaseType.SqlServer)

            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    InitialCatalog = ""
                };

                var rawConnection = new SqlConnection(builder.ConnectionString);
                var cmd = new SqlCommand
                {
                    Connection = rawConnection
                };
                rawConnection.Open();

                cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Test') CREATE DATABASE Test;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "USE Test;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
                    BEGIN
                        CREATE TABLE Users (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            UserName NVARCHAR(100) NOT NULL,
                            NickName NVARCHAR(100) NOT NULL,
                            DepartmentId INT NULL,
                            State INT NOT NULL DEFAULT 1,
                            CreatedTime DATETIME NOT NULL DEFAULT GETDATE(),
                            UpdatedTime DATETIME NOT NULL DEFAULT GETDATE()
                        );
                    END
                ";
                cmd.ExecuteNonQuery();
            }
        }
    }
}