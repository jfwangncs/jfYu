

### 缓存

```
Install-Package jfYu.Core.Cache
```

支持内存、redis缓存,如果设置为redis,必须配置redis的连接

```
{
  "Cache": {
    "Type": "Redis", //缓存类型Memory,Redis
    "KeySuffix": "" //缓存key前缀
  },
   "Redis": {
    "EndPoints": [
      {
        "Host": "jfwang.tpddns.cn", //地址
        "Port": "31002" //端口

      },
      {
        "Host": "jfwang.tpddns.cn", //地址
        "Port": "31003" //端口     
      }
    ],
    "Password": "Ncscd1111", //密码
    "KeySuffix": "", //redis key前缀（缓存服务不会采用此前缀）
    "Timeout": "5000", //超时时间（毫秒） 默认5秒
    "DbIndex": "15" //数据库 默认为0
  }
}
```
通过IOC根据配置文件直接返回对应的缓存操作对象。

```
var ContainerBuilder = new ContainerBuilder();
var builder = new ConfigurationBuilder().AddConfigurationFile("appsettings.json", optional: true, reloadOnChange: true); //注入配置文件
var Configuration = builder.Build();
ContainerBuilder.AddCache(); //ioc注入
ContainerBuilder.AddCacheAsProperties(); //属性注入
var CacheService = icon.Resolve<ICache>(); //解析
//添加缓存
CacheService.Add("testkey1", "testvalue1");
//添加缓存加上过期时间
CacheService.Add("testkey1", "testvalue1", 3)
CacheService.Add("testkey1", "testvalue1", TimeSpan.FromSeconds(3))
//判断是否存在缓存
CacheService.Has("testkey1")
//获取缓存
CacheService.GetString("testkey2")
CacheService.GetInt("testkey6")
CacheService.Get<TestModel>("testkey8")
CacheService.Get("testkey1").ToString()
//批量获取缓存
string[] list = { "testkey1", "testkey2" };
var listvalue = CacheService.GetRange(list);
//修改缓存
CacheService.Replace("testkey1", "testvalue11")
CacheService.Replace("testkey2", "testvalue22", 3)
CacheService.Replace("testkey2", "testvalue22",TimeSpan.FromSeconds(3))
//删除缓存
CacheService.Remove("testkey6")
//批量删除缓存
string[] list = { "testkey1", "testkey2" };
var rlc = CacheService.RemoveRange(list);//返回操作失败的key

```
