
### redis

Install

```
Install-Package jfYu.Core.Redis
```


Configure

```
"Redis": {
    "EndPoints": [
      {
        "Host": "127.0.0.1",
        "Port": 6379
      }
    ],
    "Password": "Password",
    "DbIndex": 0,
    "Timeout": 5000,
    "Ssl": false
  }
```


```
//injection
builder.Services.AddRedisService((new RedisConfiguration() { EndPoints = [new RedisEndPoint() { Host = "127.0.0.1" }] });  

redis.SetAsync("x", "y");
redis.GetAsync("x");
redis.RemoveAsync("x");
redis.Database.HashSet("x", new StackExchange.Redis.HashEntry[] { new StackExchange.Redis.HashEntry("n", "y") });

```


