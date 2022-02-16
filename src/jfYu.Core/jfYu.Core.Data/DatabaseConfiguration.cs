using System;
using System.Collections.Generic;


namespace jfYu.Core.Data
{
    /// <summary>
    /// 数据库连接
    /// </summary>
    public class DatabaseConfiguration
    {
        /// <summary>
        /// 数据量类型 SqlServer/MySql
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// 主数据库连接
        /// </summary>
        public string MasterConnectionString { get; set; }


        /// <summary>
        /// 从数据库检查
        /// </summary>
        public bool SlaveCheck { get; set; }

        /// <summary>
        /// 从数据库连接
        /// </summary>
        public List<SlaveConnectionString> SlaveConnectionStrings { get; set; } = new List<SlaveConnectionString>();
    }

    /// <summary>
    /// 从数据库
    /// </summary>
    public class SlaveConnectionString
    { /// <summary>
      /// 数据量类型 SqlServer/MySql
      /// </summary>
        public DatabaseType DatabaseType { get; set; }

        /// <summary>
        /// 链接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public int Weight { get; set; }
    }
}
