using jfYu.Core.Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace jfYu.Core.Data.Extension
{
    public static class DbContextServiceExtensions
    {
        private static readonly object _syncLock = new();
        private static DatabaseConfig? _config;

        /// <summary>
        /// injection
        /// </summary>  
        public static void AddJfYuDbContextService<T>(this IServiceCollection services, JfYuDBConfig configuration) where T : DbContext, IJfYuDbContextService
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentException.ThrowIfNullOrEmpty(configuration.ConnectionString);

            SetConfiglValue(configuration);
            try
            {
                services.AddDbContext<T>((q, masterOptBuilder) =>
                {
                    var config = GetConfigValue();
                    ArgumentNullException.ThrowIfNull(config);
                    switch (config.DatabaseType)
                    {
                        case DatabaseType.Mysql:
                            masterOptBuilder.UseMySql(config.ConnectionString, ServerVersion.AutoDetect(config.ConnectionString)).EnableDetailedErrors();
                            break;
                        case DatabaseType.Sqlite:
                            masterOptBuilder.UseSqlite(config.ConnectionString).EnableDetailedErrors();
                            break;
                        default:
                        case DatabaseType.SqlServer:
                            masterOptBuilder.UseSqlServer(config.ConnectionString).EnableDetailedErrors();
                            break;
                    }
                }, ServiceLifetime.Transient, ServiceLifetime.Transient);


                if (configuration.ReadOnlyConfigs != null && configuration.ReadOnlyConfigs.Count > 0)
                {
                    services.AddScoped<IContextRead, T>(services =>
                    {
                        int index = new Random().Next(configuration.ReadOnlyConfigs.Count);
                        var readonlyConfig = configuration.ReadOnlyConfigs[index];
                        SetConfiglValue(readonlyConfig);
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });

                }
                else
                {
                    services.AddScoped<IContextWrite, T>(services =>
                    {
                        SetConfiglValue(configuration);
                        return services.GetService<T>() ?? throw new ArgumentNullException(nameof(T));
                    });
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

        public static DatabaseConfig? GetConfigValue()
        {
            lock (_syncLock)
            {
                return _config;
            }
        }

        public static void SetConfiglValue(DatabaseConfig value)
        {
            lock (_syncLock)
            {
                _config = value;
            }
        }

    }
}
