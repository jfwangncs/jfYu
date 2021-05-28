

###  代码轻量级读写分离功能

支持MySql和SqlServer,如无从数据库可不配置，自动使用主数据库

```
Install-Package jfYu.Core.Data
```

配置文件

```
 "ConnectionStrings": {
    "DatabaseType": "SqlServer", //MySql  
    "MasterConnectionString": "Data Source = 127.0.0.1,9004; database = dbtest; User Id = sa; Password = 123456;",
    "SlaveCheck": false,
    "SlaveConnectionStrings": [
      {
        "ConnectionString": "Data Source = 127.0.0.1,9005; database = dbtest; User Id = sa; Password = 123456;",
        "Weight": 2
      },
      {
        "ConnectionString": "Data Source = 127.0.0.1,9006; database = dbtest; User Id = sa; Password = 123456;",
        "Weight": 8
      }
    ]

    //"DatabaseType": "MySql", 
    //"MasterConnectionString": "server=127.0.0.1;userid=root;pwd=123456;port=9001;database=dbtest;",
    //"SlaveConnectionStrings": [
    //  {
    //    "ConnectionString": "server=127.0.0.1;userid=root;pwd=123456;port=9002;database=dbtest;",
    //    "Weight": 2
    //  },
    //  {
    //    "ConnectionString": "server=127.0.0.1;userid=root;pwd=123456;port=9003;database=dbtest;",
    //    "Weight": 8
    //  }
    //]
  }
```
创建自己的DbContext

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

    public class DataContext : DbContext
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
            var connectionString = Environment.GetEnvironmentVariable("EFCORETOOLSDB");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("The connection string was not set in the 'EFCORETOOLSDB' environment variable.");
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new DataContext(optionsBuilder.Options);
        }
    }
```
迁移数据,在控制台或者cmd命令行下执行
```
//配置迁移数据库连接字符串
$env:EFCORETOOLSDB="Data Source = xxx; database = test; User Id = sa; Password = xxx;";
//新建迁移
dotnet ef migrations add init
//应用迁移(也可在代码中进行迁移)
dotnet ef migrations database update

```


DataContext 为自己创建的DbContext对象

```
var containerBuilder = new ContainerBuilder();
var builder = new ConfigurationBuilder().AddConfigurationFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Build();
containerBuilder.AddDbContextService<DataContext>();
var container = containerBuilder.Build();
var db=container.Resolve<IDbContextService<DataContext>>();
db.Master.Database.Migrate(); //代码应用迁移

```
使用

```
////master
if (db.Master.Database.GetPendingMigrations().Any())
    db.Master.Database.Migrate();

//slave
if (db.Slave.Database.GetPendingMigrations().Any())
    db.Slave.Database.Migrate();

//写
db.Master.Users.Add(new User() { Id = 1, Name = 2 });
db.Master.SaveChanges();
//读
db.Slave.Users.ToList();
db.Slave.Users.Count();
```
