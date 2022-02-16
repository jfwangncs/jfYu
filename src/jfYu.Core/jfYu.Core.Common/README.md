
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

##### NLog日志

准备NLog配置文件
```
<?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload = "true"
      internalLogFile="logs/SysNlog.log"
  >
  <targets>
    <target xsi:type="File" name="ErrorLog" fileName="logs\${shortdate}-error.log"
            layout="${longdate}|${uppercase:${level}}|${message}|${event-context:item=type}|${event-context:item=message}|${event-context:item=ip}|${event-context:item=url}| ${exception:format=tostring}" />
    <target xsi:type="File" name="TraceLog" fileName="logs\${shortdate}-trace.log"
          layout="${longdate}|${uppercase:${level}}|${message}|${event-context:item=type}|${event-context:item=message}|${event-context:item=ip}|${event-context:item=url}| ${exception:format=tostring}" />
    <target name="Console" xsi:type="Console" layout="${longdate}|${message}"/>
    <target name="asyncdb" xsi:type="AsyncWrapper" queueLimit="5000" overflowAction="Discard">
      <target xsi:type="Database"  name="db" connectionString="${var:NLOG_CONNECTION_STRING}">
        <commandText>
          INSERT INTO [dbo].[Logs]([State],[CreateTime],[UpdateTime],[Type],[Message],[Content],[UserId],[TrueName],[IP],[Url])
          VALUES(1,@CreateDate,@CreateDate,@Type,@Message,@Content,@UserId,@TrueName,@IP,@Url);
        </commandText>
        <parameter name="@CreateDate" layout="${longdate}"/>
        <parameter name="@Type" layout="${event-context:item=type}"/>
        <parameter name="@Message" layout="${message}"/>
        <parameter name="@Content" layout="${event-context:item=content}"/>
        <parameter name="@UserId" layout="${event-context:item=userid}"/>
        <parameter name="@TrueName" layout="${event-context:item=truename}"/>
        <parameter name="@IP" layout="${event-context:item=ip}"/>
        <parameter name="@Url" layout="${event-context:item=url}"/>

      </target>
    </target>
  </targets>
  <rules>
    <logger name="*" level="Debug" writeTo="Console" />
    <logger name="*" level="Trace" writeTo="TraceLog" />
    <logger name="*" level="Info" writeTo="db" />
    <logger name="*" minlevel="Warn" writeTo="ErrorLog,db" />

  </rules>
</nlog>

```
IOC注入

```
  var config = new ConfigurationBuilder().AddConfigurationFile("SqlServer.json", true, true);
  containerBuilder.AddNLog("nLog.config");
  containerBuilder1.AddNLog("nLog.config", AppConfig.Configuration.GetSection("ConnectionStrings:MasterConnectionString").Value);\\带插入数据库的日志

```

使用

```
   var logger = _container.Resolve<ILogger>();

   //普通写入
   logger.Trace("x2");
   logger.Log(new LogEventInfo(logLevel, logType, message));

   //扩展方法写入
   logger.ALog(LogLevel.Info, "错误", "异常");

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