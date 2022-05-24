
### 读取配置以及一些公共方法

##### 读取配置文件
```
Install-Package jfYu.Core.Common
```
 

##### 加解密
 using jfYu.Core.Common.Utilities


```
//Aes加解密
"test".AesEncrypt().AesEncrypt();
//Des加解密
"test".DesEncrypt().DesEncrypt();
//Ras加解密
Rsa.Encrypt("test", "d://pems/RSA.Pub")
Rsa.Decrypt("testEncryptString", "d://pems/RSA.Pub");

//SHA
//MD5
"test".SHAmd5Encrypt();
//SHA1
"test".SHA1Encrypt();
//SHA256
"test".SHA256Encrypt();
//SHA384
"test".SHA384Encrypt();
//SHA512
"test".SHA384Encrypt();

```

##### DataTable转List

```
var dt=new DataTable();
var list=dt.ToModels<T>();

var dr=dt.Rows[0];
T model=dt.ToModel<T>();

```

##### 序列化

```
var res=Serializer.Deserialize<T>(json)
var res=Serializer.Serialize(object)

```

##### unix时间戳

```
UnixTime.GetUnixTime();//获取当前时间戳

UnixTime.GetUnixTime(DateTime.Now);//获取指定时间戳

UnixTime.GetDateTime(unixtime);//时间戳转DateTime

```

##### 写入文件日志

```
//可手动IOC注入或者直接new
//log文件程序根目录Logs每天一个
 WriteFileLog log=new WriteFileLog();
 log.WriteLog("xxxx");//写入log
 log.WriteLog("xxxx","logxxx");//写入log到相应名称的log文件中

```


##### AutoMapper


```
//注入
var containerBuilder = new ContainerBuilder();
containerBuilder.AddAutoMapper(cfg =>
{
    cfg.CreateMap<User, UserProfileViewModel>().ForMember(q => q.AllName, opt => opt.MapFrom(q => q.NickName + q.UserName));
});

var c = containerBuilder.Build();
var am = c.Resolve<IMapper>();

//使用
  var u = new User() { UserName = "x", NickName = "y" };
  var vu = am.Map<UserProfileViewModel>(u);
```