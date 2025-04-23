namespace jfYu.Core.Data.Constant
{
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
        public string ConnectionString { get; set; } = string.Empty;
    }
}