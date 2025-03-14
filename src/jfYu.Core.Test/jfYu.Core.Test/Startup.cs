using jfYu.Core.Data.Constant;
using jfYu.Core.Data.Extension;
using jfYu.Core.Office;
using jfYu.Core.Redis.Extensions;
using jfYu.Core.Redis.Options;
using jfYu.Core.Test.MemoryDB;
using jfYu.Core.Test.Models.Entity;
using jfYu.Core.Test.Models.Service;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;

namespace jfYu.Core.Test
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddRedisService(options =>
            {
                options.EndPoints.Add(new RedisEndPoint { Host = "localhost" });
                options.SSL = false;
                options.DbIndex = 1;
                options.Prefix = "Mytest:";
                options.EnableLogs = true;
                options.UsingNewtonsoft(options =>
                {
                    options.MaxDepth = 12;
                });
            });
            services.AddJfYuExcel(q => { q.SheetMaxRecord = 10; });
            services.AddJfYuWord();
            services.AddDbContext<TestDbContext>();

            var databaseType = DatabaseType.SqlServer;

            var connectionString = "Data Source = 127.0.0.1; database = Test; User Id = sa; Password = StrongP@ssw0rd!;Encrypt=True;TrustServerCertificate=True;";

            services.AddJfYuDbContextService<DataContext>(q =>
            {
                q.DatabaseType = databaseType;

                q.ConnectionString = connectionString;
            });

            services.AddScoped<IUserService, UserService>();
            InitializeDatabase(databaseType, connectionString);
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
                var cmd = new MySqlCommand();
                cmd.Connection = rawConnection;
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
                var cmd = new SqlCommand();
                cmd.Connection = rawConnection;
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
