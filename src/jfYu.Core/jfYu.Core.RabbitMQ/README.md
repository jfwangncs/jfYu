
### RabbitMQ

```
Install-Package jfYu.Core.RabbitMQ
```
封装的RabbitMQ工具，队列，数据持久化，支持发送和接受重连，支持常用的5种模式

配置rabbit连接
```
 "RabbitMQ": {
    "HostName": "127.0.0.1",
    "UserName": "jfwang",
    "Password": "123456",
    "VirtualHost": "/",
    "HeartBeat": "60",
    "Port": "9808"
  }
```

ioc注入
```
var builder = new ConfigurationBuilder()
        .AddConfigurationFile("appsettings.json", optional: true, reloadOnChange: true);
var Configuration = builder.Build();
var con = new ContainerBuilder();
con.AddRabbitMQService();
```

发送
```
var icon = con.Build();
SendEventMessage(icon.Resolve<IRabbitMQService>());
private static void SendEventMessage(IRabbitMQService rabbitMQServer)
        {
            for (var i = 1; i < 100; i++)
            {
                var originObject = new testclass()
                {
                    id = i,
                    name = "姓名" + i

                };

                rabbitMQServer.Send("changeNametest3","direct", originObject);

                Console.WriteLine($"发送{i}");
                Thread.Sleep(2000);
            }
        }
```

接收
```
var mq = icon.Resolve<IRabbitMQService>();
mq.Receive("queName", "changeNametest3", "direct", q =>
{
   Thread.Sleep(4000);
   Console.WriteLine(q);
});
```
