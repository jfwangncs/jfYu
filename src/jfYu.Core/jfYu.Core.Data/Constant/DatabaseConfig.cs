using System.Collections.Generic;

namespace jfYu.Core.Data.Constant
{

    public class JfYuDatabaseConfig : DatabaseConfig
    {
        public List<DatabaseConfig> ReadOnlyDatabases { get; set; } = [];
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
