
### redis

安装

```
Install-Package jfYu.Core.Redis
```
使用
```
 var ContainerBuilder = new ContainerBuilder();
var builder = new ConfigurationBuilder()
    .AddConfigurationFile("CacheRedis.json", optional: true, reloadOnChange: true);
_ = builder.Build();
ContainerBuilder.AddRedisService();
var c = ContainerBuilder.Build();
var redis = c.Resolve<RedisService>();
redis.Set("x", "y");
redis.Get("x");
redis.Remove("x");
redis.Database.HashSet("x", new StackExchange.Redis.HashEntry[] { new StackExchange.Redis.HashEntry("n", "y") });

```


