using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.Common;

#if NET8_0_OR_GREATER
using JfYu.Data.Constant;
using JfYu.Data.Extension;
using JfYu.UnitTests.Models.Entity;
#endif




namespace JfYu.UnitTests
{
    public static class Common
    {
#if NET8_0_OR_GREATER
        public static IServiceCollection AddDataContextServices(this IServiceCollection services)
        {

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .Build(); 

            var dbName = "TestDb_" + Guid.NewGuid().ToString("N");         

            services.AddJfYuDbContextService<DataContext>(options =>
            {
                configuration.GetSection("JfYuConnectionStrings").Bind(options);
                var builder = new DbConnectionStringBuilder
                {
                    ConnectionString = options.ConnectionString
                };
                builder["Database"] = dbName;
                options.ConnectionString = builder.ConnectionString;
            });
            return services;
        }
#endif
        public static void InitializeDatabase(bool isMyql, string connectionString)
        {
            if (isMyql)
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
                        Status INT NOT NULL DEFAULT 1,
                        CreatedTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        UpdatedTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
                    );
                ";
                cmd.ExecuteNonQuery();
                rawConnection.Close();
            }
            else
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
                            Status INT NOT NULL DEFAULT 1,
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
