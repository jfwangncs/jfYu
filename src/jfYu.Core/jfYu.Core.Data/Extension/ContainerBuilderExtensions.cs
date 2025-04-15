using jfYu.Core.Data.Constant;
using jfYu.Core.Data.Context;
using jfYu.Core.Data.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace jfYu.Core.Data.Extension
{
    /// <summary>
    ///  Adds JfYuDbContext services extensions
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds and configures the JfYu DbContext service to the dependency injection container.
        /// </summary>
        /// <typeparam name="T">The type of DbContext to configure (must inherit from DbContext).</typeparam>
        /// <param name="services">The IServiceCollection to add the DbContext service to.</param>
        /// <param name="setupAction">An action to configure the JfYuDatabaseConfig options.</param>
        /// <returns>The modified IServiceCollection instance for chaining.</returns>
        public static IServiceCollection AddJfYuDbContextService<T>(this IServiceCollection services, Action<JfYuDatabaseConfig> setupAction) where T : DbContext
        {
            ArgumentNullException.ThrowIfNull(setupAction, nameof(setupAction));

            var options = new JfYuDatabaseConfig();
            setupAction.Invoke(options);

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new ArgumentNullException("Main connectionString can't be empty.", nameof(options.ConnectionString));

            if (options.ReadOnlyDatabases.Any(db => string.IsNullOrWhiteSpace(db.ConnectionString)))
                throw new ArgumentNullException("One or more ReadOnlyDatabases have an empty connection string.", nameof(options.ReadOnlyDatabases));

            services.AddDbContext<T>(q =>
            {
                GetDbContextOptions<T>(options, q);
            });

            for (int i = 0; i < options.ReadOnlyDatabases.Count; i++)
            {
                var dbConfig = options.ReadOnlyDatabases[i];
                services.AddKeyedScoped($"{options.JfYuReadOnly}{i}", (provider, t) =>
                {
                    var dbContextOptions = GetDbContextOptions<T>(dbConfig);
                    return (T)Activator.CreateInstance(typeof(T), dbContextOptions)!;
                });
            }
            services.AddScoped(provider =>
            {
                if (options.ReadOnlyDatabases.Count < 1)
                    return new ReadonlyDBContext<T>(provider.GetRequiredService<T>());
                var randomKey = Random.Shared.Next(options.ReadOnlyDatabases.Count);
                return new ReadonlyDBContext<T>(provider.GetRequiredKeyedService<T>($"{options.JfYuReadOnly}{randomKey}"));
            });

            services.AddScoped(typeof(ReadonlyDBContext<>));

            services.AddScoped(typeof(IService<,>), typeof(Service<,>));

            return services;
        }
     
        private static DbContextOptions GetDbContextOptions<T>(DatabaseConfig config, DbContextOptionsBuilder? opt = null) where T : DbContext
        {
            var optionsBuilder = opt ?? new DbContextOptionsBuilder<T>();

            switch (config.DatabaseType)
            {
                case DatabaseType.MySql:
                    optionsBuilder.UseMySql(config.ConnectionString, ServerVersion.AutoDetect(config.ConnectionString));
                    break;
                case DatabaseType.Sqlite:
                    optionsBuilder.UseSqlite(config.ConnectionString);
                    break;
                default:
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(config.ConnectionString);
                    break;
            }

            return optionsBuilder.Options;
        }

    }
}
