using System.Collections.Generic;

namespace jfYu.Core.Data.Constant
{
    /// <summary>
    /// JfYu Database Configuration
    /// </summary>
    public class JfYuDatabaseConfig : DatabaseConfig
    {
        /// <summary>
        /// List of read-only database configurations
        /// </summary>
        public List<DatabaseConfig> ReadOnlyDatabases { get; set; } = [];

        /// <summary>
        /// Read-only database key identifier
        /// </summary>
        public string JfYuReadOnly = nameof(JfYuReadOnly);
    }

    /// <summary>
    /// Database Configuration
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// SqlServer/MySql/sqlite
        /// </summary>
        public DatabaseType DatabaseType { get; set; } = DatabaseType.SqlServer;

        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; set; } = "";
    }
}