
### mongodb


```
Install-Package jfYu.Core.MongoDB
```
配置文件

```
 "MongoDB": {
    "MongoUrl": "mongodb+srv://jfwang:xxx@xxx.net/admin",
    "DbName": "nba"
  }

```

```
//IOC注入
var builder = new ConfigurationBuilder()
    .AddConfigurationFile("MongoDB.json", optional: true, reloadOnChange: true);
var cb = new ContainerBuilder();
cb.AddMongoDB();
var con = cb.Build();
var mongoDBService = con.Resolve<MongoDBService>();
Data d1 = new Data() { name = "姓名1", age = "18", sex = "男" };
Data d2 = new Data() { name = "姓名2", age = "19", sex = "男" };
Data d3 = new Data() { name = "姓名3", age = "20", sex = "女" };
//添加
d1 = mongoDBService.Insert(d1);
//批量添加
mongoDBService.InsertBatch(new List<Data>() { d3, d4 });
//删除
mongoDBService.Delete<Data>(d11.Id.ToString());
mongoDBService.SoftDelete<Data>(d11.Id.ToString());
//修改
 mongoDBService.Modify<Data>(d11.Id.ToString(), "name", "姓名1改后");
 mongoDBService.Update(d11);
//获取单个
var d111=ongoDBService.QueryOne<Data>(q => q.Id == d11.Id);
//列表
var list=mongoDBService.QueryCollection<Data>(q => q.sex == "女")
//分页
 var lc = mongoDBService.QueryCollection<Data>().ToPaging(new QueryModel() { PageIndex = 1, PageSize = 2 });

  ```
