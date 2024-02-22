using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace jfYu.Core.Data.Extension
{
    public static class DbContextServiceExtensions
    {
        private static readonly AsyncLocal<DatabaseConfig> config = new();

        /// <summary>
        /// injection
        /// </summary>  
        public static void AddJfYuDbContextService<T>(this IServiceCollection services, JfYuDBConfig configuration) where T : DbContext, IJfYuDbContextService
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentException.ThrowIfNullOrEmpty(configuration.ConnectionString);

            config.Value = configuration;
            try
            {
                services.AddDbContext<T>(masterOptBuilder =>
                {
                    var dbConfig = config.Value ?? throw new ArgumentNullException(nameof(configuration));
                    switch (dbConfig.DatabaseType)
                    {
                        case DatabaseType.Mysql:
                            masterOptBuilder.UseMySql(dbConfig.ConnectionString, ServerVersion.AutoDetect(dbConfig.ConnectionString)).EnableDetailedErrors();
                            break;
                        case DatabaseType.Sqlite:
                            masterOptBuilder.UseSqlite(dbConfig.ConnectionString).EnableDetailedErrors();
                            break;
                        default:
                        case DatabaseType.SqlServer:
                            masterOptBuilder.UseSqlServer(dbConfig.ConnectionString).EnableDetailedErrors();
                            break;
                    }
                }, ServiceLifetime.Transient, ServiceLifetime.Transient);


                if (configuration.ReadOnlyConfigs != null && configuration.ReadOnlyConfigs.Count > 0)
                {
                    services.AddScoped<IContextRead, T>(services =>
                    {
                        int index = new Random().Next(configuration.ReadOnlyConfigs.Count);
                        var readonlyConfig = configuration.ReadOnlyConfigs[index];
                        config.Value = readonlyConfig;
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });

                }
                else
                {
                    services.AddScoped<IContextRead, T>(services =>
                    {
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });
                }

                services.AddScoped(typeof(ReadonlyDBContext<>));

                //register service
                services.AddScoped(typeof(IService<,>), typeof(Service<,>));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
