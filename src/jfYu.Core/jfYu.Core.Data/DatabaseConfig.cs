using jfYu.Core.Data.Model;
using System.Collections.Generic;

namespace jfYu.Core.Data
{

    public class JfYuDBConfig : DatabaseConfig
    {
        public List<DatabaseConfig> ReadOnlyConfigs { get; set; } = [];
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
