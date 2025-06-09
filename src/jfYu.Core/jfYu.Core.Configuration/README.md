
### <a href="#Configuration">Configuration配置管理</a>
```
1、自动读取目录下配置文件、根据环境变量读取配置文件、或读取单个配置文件。
2、选项模式查找逻辑类名以Options结尾，则去除Options字符串否则则为类名。

```
Nuget安装

```
Install-Package jfYu.Core.Configuration
```

使用

```
//加载配置
new ConfigurationBuilder().AddConfigurationFiles(); //加载所有
new ConfigurationBuilder().AddConfigurationFiles(new MyHostEnvironment());//按环境变量加载
new ConfigurationBuilder().AddConfigurationFile("CacheRedis.json", true, true);//加载单独配置文件

//读取配置
//配置模式
AppConfig.Configuration["Cache:Type"]
AppConfig.Configuration.GetSection("Redis").GetSection("Timeout").Value
//选项模式 Class继承IConfigurableOptions

public class RedisOptions : IConfigurableOptions
{

	public List<RedisEndPoint> EndPoints { get; set; }
	/// <summary>
	/// 密码
	/// </summary>

	public string Password { get; set; }

	/// <summary>
	/// 数据库index
	/// </summary>

	public int DbIndex { get; set; } = 0;

	/// <summary>
	/// 超时时间（毫秒）
	/// </summary>

	public int Timeout { get; set; } = 5000;

}

public class RedisEndPoint
{
	/// <summary>
	/// 主机
	/// </summary>
	public string Host { get; set; }

	/// <summary>
	/// 端口
	/// </summary>
	public int Port { get; set; }
}
//IOC注入
 var services = new ServiceCollection();
 services.AddConfigurableOptions<RedisOptions>();
 //IOC读取
 ContainerBuilder containerBuild = new ContainerBuilder();
 containerBuild.Populate(services);
 containerBuild.RegisterType<OptionServer>();
 var container = containerBuild.Build();
 var options = container.Resolve<OptionServer>();

```