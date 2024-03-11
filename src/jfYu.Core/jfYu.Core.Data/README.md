

###  代码轻量级读写分离功能

MySql,SqlServer,Sqlite 

```
Install-Package jfYu.Core.Data
```

配置文件

```
 "ConnectionStrings": {
    "DatabaseType": "SqlServer",
    "ConnectionString": "Data Source = 127.0.0.1,9004; database = dbtest; User Id = sa; Password = 123456;"
    "ReadOnlyConfigs": [
      {
        "DatabaseType": "MySql", 
        "ConnectionString": "server=127.0.0.1;userid=root;pwd=123456;port=9001;database=dbtest;"
      },
      {
        "DatabaseType": "Sqlite",  
        "ConnectionString": "Data Source= data/m2.db;Password = 123456;"
    ]   
  }
```

创建DbContext

```
    public class User : BaseEntity
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [DisplayName("登录名"), Required, MaxLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [DisplayName("昵称"), Required, MaxLength(100)]
        public string NickName { get; set; }       

        /// <summary>
        /// 所属部门编号
        /// </summary>
        [DisplayName("所属部门")]
        public Guid? DepartmentId { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        public virtual Department Department { get; set; }

    }

    public class Department : BaseEntity
    {        
        /// <summary>
        /// 名称
        /// </summary>
        [DisplayName("名称"), Required]
        public string Name { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        [DisplayName("简称"), Required]
        public string SubName { get; set; }

        /// <summary>
        /// 上级部门编号
        /// </summary>
        [DisplayName("上级部门")]
        public int? SuperiorId { get; set; }

        /// <summary>
        /// 上级部门
        /// </summary>
        [DisplayName("上级部门")]
        public virtual Department Superior { get; set; }

        /// <summary>
        /// 部门人员
        /// </summary>
        [DisplayName("部门人员")]
        public virtual List<User> Users { get; set; }
      
    }

    public class DataContext : DbContext,IJfYuDbContextService
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }

    }

  //设计时工厂
    class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("EFConString");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("The connection string was not set in the 'EFConString' environment variable.");
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(connectionString);
            //optionsBuilder.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString));
            return new DataContext(optionsBuilder.Options);
        }
    }
```
迁移数据
```
//配置迁移数据库连接字符串
$env:EFConString="Data Source = xxx; database = test; User Id = sa; Password = xxx;";
//新建迁移
dotnet ef migrations add init
//应用迁移(也可在代码中进行迁移)
dotnet ef  database update

```

注册

```

  services.AddJfYuDbContextService<DataContext>(new JfYuDBConfig() { DatabaseType = DatabaseType.Sqlite, ConnectionString = "Data Source= data/m1.db;" });

```
使用

```
//master
var _masterContext = serviceProvider.GetService<DataContext>();

//readonly
serviceProvider.GetService<ReadonlyDBContext<DataContext>>();

//service
var _companyService=serviceProvider.GetService<IService<Company, DataContext>>();


await _companyService.AddAsync(new Company() { Age = 33, Name = "test" }
await _companyService.UpdateAsync(data)
await _companyService.RemoveAsync(q => q.ID.Equals(data.ID))
_companyService.GetList(q => q.Name == "test124")

```
