### Cache

```
Install-Package jfYu.Core.Cache
```

```
//injection
builder.Services.AddCacheService(); //memory
builder.Services.AddCacheService("redis connection string");// distribution 
//add
_cacheService.AddAsync("testkey1", "testvalue1");

//add with expire timee
_cacheService.AddAsync("testkey1", "testvalue1", 3)
_cacheService.AddAsync("testkey1", "testvalue1", Datetime.Now.AddSeconds(3))
 
//get
_cacheService.GetAsync("testkey2")
_cacheService.GetIntAsync("testkey6")
_cacheService.GetAsync<TestModel>("testkey8") 
/ 
//remove
CacheService.RemoveAsync("testkey6")
CacheService.RemoveAllAsync("testkey6")

```
