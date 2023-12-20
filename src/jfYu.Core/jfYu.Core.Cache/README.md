

### 缓存

```
Install-Package jfYu.Core.Cache
```

支持内存、redis缓存,如果设置为redis,必须配置redis的连接

 
通过IOC根据配置文件直接返回对应的缓存操作对象。

```
//ioc注入
builder.Services.AddCacheService(); //memory
builder.Services.AddCacheService(true,"redis connection string");
//添加缓存
CacheService.Add("testkey1", "testvalue1");
//添加缓存加上过期时间
CacheService.Add("testkey1", "testvalue1", 3)
CacheService.Add("testkey1", "testvalue1", Datetime.Now.AddSeconds(3))
 
//获取缓存
CacheService.GetAsync("testkey2")
CacheService.GetIntAsync("testkey6")
CacheService.Get<TestModel>("testkey8") 
/ 
//删除缓存
CacheService.Remove("testkey6")
 

```
